using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Brainf_ck_sharp_UWP.DataModels;
using Brainf_ck_sharp_UWP.DataModels.SQLite;
using Brainf_ck_sharp_UWP.DataModels.SQLite.Enums;
using Brainf_ck_sharp_UWP.Helpers;
using Brainf_ck_sharp_UWP.Helpers.Extensions;
using JetBrains.Annotations;
using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Platform.WinRT;

namespace Brainf_ck_sharp_UWP.SQLiteDatabase
{
    /// <summary>
    /// A simple class that manages the local database in the app
    /// </summary>
    public sealed class SQLiteManager
    {
        #region Constants and parameters

        /// <summary>
        /// Gets the name of the local database
        /// </summary>
        private const string DatabaseFileName = "SourceCodes.db";

        /// <summary>
        /// Gets the name of the roaming database file name
        /// </summary>
        private const string RoamingDatabaseFileName = "RoamingCodes.db";

        /// <summary>
        /// Gets the path of the clean database
        /// </summary>
        private const string CleanDatabaseUri = "ms-appx:///Assets/Misc/SourceCodesTemplateDatabase.db";

        /// <summary>
        /// Gets the path of folder that contains the sample files
        /// </summary>
        private static string SampleFilesPath { get; } = $@"{Package.Current.InstalledLocation.Path}\Assets\Samples\";

        /// <summary>
        /// The async connection to the local database in use
        /// </summary>
        private SQLiteAsyncConnection DatabaseConnection { get; set; }

        /// <summary>
        /// Semaphore to better synchronize concurrent accesses to the database
        /// </summary>
        private readonly SemaphoreSlim DatabaseSemaphore = new SemaphoreSlim(1);

        #endregion

        /// <summary>
        /// Gets the public instance to use
        /// </summary>
        public static SQLiteManager Instance { get; } = new SQLiteManager();

        #region Initialization

        // Private constructor to prevent the app from spawning multiple instances
        private SQLiteManager() { }

        // Prepares a database connection to the target file
        private static SQLiteAsyncConnection PrepareConnection([NotNull] StorageFile file)
        {
            try
            {
                SQLitePlatformWinRT platform = new SQLitePlatformWinRT();
                SQLiteConnectionString connectionString = new SQLiteConnectionString(file.Path, true);
                SQLiteConnectionWithLock connection = new SQLiteConnectionWithLock(platform, connectionString);
                return new SQLiteAsyncConnection(() => connection);
            }
            catch
            {
                // Whops!
                return null;
            }
        }

        /// <summary>
        /// Makes sure the exceptions database is open and connected
        /// </summary>
        private async Task EnsureDatabaseConnectionAsync()
        {
            await DatabaseSemaphore.WaitAsync();
            if (DatabaseConnection == null)
            {
                // Get the local database file
                if (!(await ApplicationData.Current.LocalFolder.TryGetItemAsync(DatabaseFileName) is StorageFile database))
                {
                    StorageFile cleanDatabase = await StorageFile.GetFileFromApplicationUriAsync(new Uri(CleanDatabaseUri));
                    database = await cleanDatabase.CopyAsync(ApplicationData.Current.LocalFolder, DatabaseFileName, NameCollisionOption.ReplaceExisting);
                }

                // Initialize the connection
                DatabaseConnection = PrepareConnection(database);
            }
            DatabaseSemaphore.Release();
        }

        /// <summary>
        /// Syncs the saved source codes to the roaming folder
        /// </summary>
        [SuppressMessage("ReSharper", "AccessToDisposedClosure")] // SQLite connection in using block
        public async Task TrySyncSharedCodesAsync()
        {
            // Initialize the local database if needed
            await EnsureDatabaseConnectionAsync();
            await DatabaseSemaphore.WaitAsync();
            try
            {
                // Try to get the roaming database
                SQLitePlatformWinRT platform = new SQLitePlatformWinRT();
                if (await ApplicationData.Current.RoamingFolder.TryGetItemAsync(RoamingDatabaseFileName) is StorageFile roaming)
                {
                    // Get the database connection
                    SQLiteConnectionString connectionString = new SQLiteConnectionString(roaming.Path, true);
                    using (SQLiteConnectionWithLock rawConnection = new SQLiteConnectionWithLock(platform, connectionString))
                    {
                        // Get the async connection and query the work items
                        SQLiteAsyncConnection connection = new SQLiteAsyncConnection(() => rawConnection);
                        List<SourceCode>
                            items = await connection.Table<SourceCode>().ToListAsync(),
                            table = await DatabaseConnection.Table<SourceCode>().ToListAsync(),
                            filtered = table.Where(item => !SamplesMap.Any(entry => entry.Uid.Equals(Guid.Parse(item.Uid)))).ToList();

                        // Function that syncs from a source collection to the target database
                        async Task SyncTablesAsync(IEnumerable<SourceCode> data, SQLiteAsyncConnection source)
                        {
                            // Iterate over all the items
                            foreach (SourceCode code in data)
                            {
                                // Try to get the same item from the source connection
                                SourceCode local = await source.Table<SourceCode>().Where(item => item.Uid == code.Uid).FirstOrDefaultAsync();
                                if (local == null)
                                {
                                    // Item not found, insert it
                                    if (await source.Table<SourceCode>().Where(item => item.Title == code.Title).FirstOrDefaultAsync() != null)
                                    {
                                        // Skip if there is another code with the same name
                                        continue;
                                    }
                                    await source.InsertAsync(code);
                                }
                                else if(
                                    
                                    // The two codes must have been edited in some way to be updated here
                                    !(code.Modified == local.Modified && code.Deleted == local.Deleted) &&
                                    
                                    // The remote code has the same delete timestamp (can also be 0) but has been edited more recently
                                    (code.ModifiedTime.CompareTo(local.ModifiedTime) > 0 && code.Deleted == local.Deleted ||
                                    
                                    // The remote code has been deleted but the remote one has been edited after that (so import that one again)
                                    local.Deleted != 0 && code.Deleted == 0 && code.ModifiedTime.CompareTo(local.DeletedTime) > 0 ||
                                    
                                    // The remote code has been edited after the last edit time for the local version (so delete the local one)
                                    local.Deleted == 0 && code.Deleted != 0 && code.DeletedTime.CompareTo(local.ModifiedTime) > 0 ||
                                    
                                    // Both the items are deleted, but the remote one has been deleted more recently (so copy its delete timestamp for future use)
                                    local.Modified == code.Modified && local.Deleted != 0 && code.Deleted != 0 && code.DeletedTime.CompareTo(local.DeletedTime) > 0))
                                {
                                    // Item modified or changed after local deletion, update it or restore the remote version
                                    if (await source.Table<SourceCode>().Where(item => item.Title == code.Title && item.Uid != code.Uid).FirstOrDefaultAsync() != null)
                                    {
                                        // Skop if the updated title is the same as another one in the table
                                        continue;
                                    }
                                    await source.UpdateAsync(code);
                                }
                            }
                        }

                        // Sync the two databases
                        await SyncTablesAsync(items, DatabaseConnection);
                        await SyncTablesAsync(filtered, connection);

                        // Execute the VACUUM commands
                        try
                        {
                            await connection.ExecuteAsync("VACUUM;");
                            await DatabaseConnection.ExecuteAsync("VACUUM;");
                        }
                        catch
                        {
                            // Who cares?
                        }
                    }
                }
                else
                {
                    // Roaming file missing, initialize it
                    StorageFile 
                        cleanDatabase = await StorageFile.GetFileFromApplicationUriAsync(new Uri(CleanDatabaseUri)),
                        database = await cleanDatabase.CopyAsync(ApplicationData.Current.RoamingFolder, RoamingDatabaseFileName);

                    // Get the database connection
                    SQLiteConnectionString connectionString = new SQLiteConnectionString(database.Path, true);
                    using (SQLiteConnectionWithLock rawConnection = new SQLiteConnectionWithLock(platform, connectionString))
                    {
                        // Copy the local codes into the roaming database
                        SQLiteAsyncConnection connection = new SQLiteAsyncConnection(() => rawConnection);
                        List<SourceCode> table = await DatabaseConnection.Table<SourceCode>().ToListAsync();
                        IEnumerable<SourceCode> candidates =
                            from code in table
                            where !SamplesMap.Any(entry => entry.Uid.Equals(Guid.Parse(code.Uid)))
                            select code;
                        await connection.InsertAllAsync(candidates);
                    }
                }
            }
            catch
            {
                // Skip!
            }
            finally
            {
                // Better not to forget about this, right?
                DatabaseSemaphore.Release();
            }
        }

        #endregion

        #region Sample codes

        /// <summary>
        /// Gets a collection of names for the code samples, and their hardcoded ID
        /// </summary>
        private static readonly IReadOnlyCollection<SampleCodeRecord> SamplesMap = new List<SampleCodeRecord>
        {
            new SampleCodeRecord("HelloWorld.txt", "Hello world!", Guid.Parse("6B4C55E6-7009-48EC-96C5-C73552D9F257")),
            new SampleCodeRecord("UnicodeValue.txt", LocalizationManager.GetResource("10768D40-5E3D-4787-9CB8-2A0ABBE26EFC"), Guid.Parse("10768D40-5E3D-4787-9CB8-2A0ABBE26EFC")),
            new SampleCodeRecord("UnicodeSum.txt", LocalizationManager.GetResource("78BAA70A-0DAF-4BB6-B09A-CDA9537D2FFF"), Guid.Parse("78BAA70A-0DAF-4BB6-B09A-CDA9537D2FFF")),
            new SampleCodeRecord("Sum.txt", LocalizationManager.GetResource("0441153F-E40A-4AEC-8373-8A552697778B"),  Guid.Parse("0441153F-E40A-4AEC-8373-8A552697778B")),
            new SampleCodeRecord("HeaderComments.txt", LocalizationManager.GetResource("63156CB7-1BD1-46EA-A705-AC2ADD4A5F11"),  Guid.Parse("63156CB7-1BD1-46EA-A705-AC2ADD4A5F11")),
            new SampleCodeRecord("ExecuteIfZero.txt", "if (x == 0) then { }",  Guid.Parse("6DABC8A8-E32C-49A1-A348-CF836FEF276D"))
        };

        // Semaphore to synchronize the sample codes loading
        private readonly SemaphoreSlim SampleCodesSemaphore = new SemaphoreSlim(1);

        // A field to keep track of the code samples loading state
        private bool _SamplesRefreshed;

        /// <summary>
        /// Refreshes the list of local code samples
        /// </summary>
        private async Task RefreshCodeSamplesAsync()
        {
            // Get the samples folder and iterate over the expected samples
            await SampleCodesSemaphore.WaitAsync();
            if (!_SamplesRefreshed)
            {
                _SamplesRefreshed = true;
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(SampleFilesPath);
                foreach (SampleCodeRecord record in SamplesMap)
                {
                    // Get the source code file and look for an existing copy in the local database
                    StorageFile file = await folder.GetFileAsync(record.Filename);
                    string code = await FileIO.ReadTextAsync(file);
                    string sUid = record.Uid.ToString();
                    SourceCode row = await DatabaseConnection.Table<SourceCode>().Where(entry => entry.Uid == sUid).FirstOrDefaultAsync();
                    if (row == null)
                    {
                        // The code was missing, create a local copy in the database
                        row = new SourceCode
                        {
                            Uid = record.Uid.ToString(),
                            Title = record.FriendlyName,
                            Code = code,
                            CreatedTime = DateTime.Now,
                            ModifiedTime = DateTime.Now
                        };
                        await DatabaseConnection.InsertAsync(row);
                    }
                    else if (!row.Code.Equals(code))
                    {
                        // Make sure the code is up to date
                        row.Code = code;
                        row.ModifiedTime = DateTime.Now;
                        await DatabaseConnection.UpdateAsync(row);
                    }
                }
            }
            SampleCodesSemaphore.Release();
        }

        #endregion

        #region Public APIs

        /// <summary>
        /// Loads the saved source codes from the app database
        /// </summary>
        public async Task<IList<GroupedSourceCodesCategory>> LoadSavedCodesAsync()
        {
            // Query the codes from the database
            await EnsureDatabaseConnectionAsync();
            await RefreshCodeSamplesAsync();
            List<SourceCode> codes = await DatabaseConnection.Table<SourceCode>().Where(code => code.Deleted == 0).ToListAsync();

            // Populate the three categories
            List<SourceCode>
                samples = new List<SourceCode>(),
                favorites = new List<SourceCode>(),
                original = new List<SourceCode>();
            foreach (SourceCode code in codes)
            {
                if (SamplesMap.Any(sample => sample.Uid.Equals(Guid.Parse(code.Uid))))
                {
                    SampleSourceCode wrapper = new SampleSourceCode
                    {
                        Uid = code.Uid,
                        Title = code.Title,
                        Code = code.Code,
                        CreatedTime = code.CreatedTime,
                        ModifiedTime = code.ModifiedTime
                    };
                    samples.AddSorted(wrapper, entry => SamplesMap.IndexOf(sample => sample.Uid.Equals(Guid.Parse(entry.Uid))));
                }
                else if (code.Favorited) favorites.AddSorted(code, entry => entry.Title);
                else original.AddSorted(code, entry => entry.Title);
            }

            // Aggregate and return the results
            return new[]
            {
                new GroupedSourceCodesCategory(SavedSourceCodeType.Favorite, favorites.ToArray()),
                new GroupedSourceCodesCategory(SavedSourceCodeType.Original, original.ToArray()),
                new GroupedSourceCodesCategory(SavedSourceCodeType.Sample, samples.ToArray())
            };
        }

        /// <summary>
        /// Saves a new source code in the app local database
        /// </summary>
        /// <param name="title">The title of the source code to save</param>
        /// <param name="code">The code to save</param>
        /// <param name="breakpoints">The list of lines with an enabled breakpoint</param>
        public async Task<AsyncOperationResult<CategorizedSourceCode>> SaveCodeAsync([NotNull] string title, [NotNull] string code, [CanBeNull] IReadOnlyCollection<int> breakpoints)
        {
            // Check the name and remove the existing deleted codes with the same name, if present
            await EnsureDatabaseConnectionAsync();
            if (!await CheckExistingName(title)) return AsyncOperationStatus.Faulted;
            await RemovePendingDeletedItemsAsync(title);

            // Create and store the new code
            SourceCode entry = new SourceCode
            {
                Uid = Guid.NewGuid().ToString(),
                Code = code,
                Title = title,
                CreatedTime = DateTime.Now,
                ModifiedTime = DateTime.Now,
                Breakpoints = breakpoints == null ? null : BitHelper.Compress(breakpoints)
            };
            await DatabaseConnection.InsertAsync(entry);
            return new CategorizedSourceCode(SavedSourceCodeType.Original, entry);
        }

        /// <summary>
        /// Saves the changes to a source code
        /// </summary>
        /// <param name="code">The source code that's being edited</param>
        /// <param name="text">The updated text to save</param>
        /// <param name="breakpoints">The list of lines with an enabled breakpoint</param>
        public async Task SaveCodeAsync([NotNull] SourceCode code, [NotNull] string text, [CanBeNull] IReadOnlyCollection<int> breakpoints)
        {
            await EnsureDatabaseConnectionAsync();
            code.Code = text;
            code.ModifiedTime = DateTime.Now;
            code.Breakpoints = breakpoints == null ? null : BitHelper.Compress(breakpoints);
            await DatabaseConnection.UpdateAsync(code);
        }

        /// <summary>
        /// Checks whether or not there is a saved source code with the same title
        /// </summary>
        /// <param name="name">The title to check</param>
        public async Task<bool> CheckExistingName([NotNull] string name)
        {
            await EnsureDatabaseConnectionAsync();
            return await DatabaseConnection.Table<SourceCode>().Where(row => row.Title == name && row.Deleted == 0).FirstOrDefaultAsync() == null;
        }

        /// <summary>
        /// Removes the deleted codes with a given title from the database
        /// </summary>
        /// <param name="title">The title of the deleted codes to removes</param>
        private async Task RemovePendingDeletedItemsAsync([NotNull] string title)
        {
            List<SourceCode> existing = await DatabaseConnection.Table<SourceCode>().Where(row => row.Title == title && row.Deleted != 0).ToListAsync();
            if (existing.Count > 0)
                foreach (SourceCode pending in existing)
                    await DatabaseConnection.DeleteAsync(pending);
        }

        /// <summary>
        /// Renames a selected source code saved in the app
        /// </summary>
        /// <param name="code">The source code to rename</param>
        /// <param name="name">The new name for the saved code</param>
        public async Task RenameCodeAsync([NotNull] SourceCode code, [NotNull] string name)
        {
            await EnsureDatabaseConnectionAsync();
            await RemovePendingDeletedItemsAsync(name);
            code.Title = name;
            await DatabaseConnection.UpdateAsync(code);
        }

        /// <summary>
        /// Delets a saved source code from the database
        /// </summary>
        /// <param name="code">The code to delete</param>
        public async Task DeleteCodeAsync([NotNull] SourceCode code)
        {
            await EnsureDatabaseConnectionAsync();
            code.DeletedTime = DateTime.Now;
            await DatabaseConnection.UpdateAsync(code);
        }

        /// <summary>
        /// Toggles the favorite status for a given code
        /// </summary>
        /// <param name="code">The target code to edit</param>
        public async Task ToggleFavoriteStatusAsync([NotNull] SourceCode code)
        {
            await EnsureDatabaseConnectionAsync();
            code.Favorited = !code.Favorited;
            await DatabaseConnection.UpdateAsync(code);
        }

        /// <summary>
        /// Tries to load the saved code with the specified id, if it exists
        /// </summary>
        /// <param name="uid">The id of the saved code to look for</param>
        [ItemCanBeNull]
        public async Task<CategorizedSourceCode> TryLoadSavedCodeAsync([NotNull] string uid)
        {
            await EnsureDatabaseConnectionAsync();
            SourceCode target = await DatabaseConnection.Table<SourceCode>().Where(code => code.Uid == uid).FirstOrDefaultAsync();
            return target == null ? null : new CategorizedSourceCode(target.Favorited ? SavedSourceCodeType.Favorite : SavedSourceCodeType.Original, target);
        }

        #endregion
    }
}
