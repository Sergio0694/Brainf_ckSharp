using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Brainf_ck_sharp_UWP.DataModels.SQLite;
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
        /// Gets the name of the ShareTarget history database file
        /// </summary>
        private const String DatabaseFileName = "SourceCodes.db";

        /// <summary>
        /// Gets the path of the clean database
        /// </summary>
        private const String CleanDatabaseUri = "ms-appx:///Assets/Misc/SourceCodesTemplateDatabase.db";

        /// <summary>
        /// Gets the path of folder that contains the sample files
        /// </summary>
        private static String SampleFilesPath { get; } = $@"{Package.Current.InstalledLocation.Path}\Assets\Samples\";

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

        /// <summary>
        /// Makes sure the exceptions database is open and connected
        /// </summary>
        private async Task EnsureDatabaseConnectionAsync()
        {
            await DatabaseSemaphore.WaitAsync();
            if (DatabaseConnection == null)
            {
                // Get the local database file
                StorageFile database = await ApplicationData.Current.LocalFolder.TryGetItemAsync(DatabaseFileName) as StorageFile;
                if (database == null)
                {
                    StorageFile cleanDatabase = await StorageFile.GetFileFromApplicationUriAsync(new Uri(CleanDatabaseUri));
                    database = await cleanDatabase.CopyAsync(ApplicationData.Current.LocalFolder, DatabaseFileName, NameCollisionOption.ReplaceExisting);
                }

                // Initialize the connection
                try
                {
                    SQLitePlatformWinRT platform = new SQLitePlatformWinRT();
                    SQLiteConnectionString connectionString = new SQLiteConnectionString(database.Path, true);
                    SQLiteConnectionWithLock connection = new SQLiteConnectionWithLock(platform, connectionString);
                    DatabaseConnection = new SQLiteAsyncConnection(() => connection);
                }
                catch
                {
                    DatabaseConnection = null;
                }
            }
            DatabaseSemaphore.Release();
        }

        #endregion

        #region Sample codes

        /// <summary>
        /// Gets a collection of names for the code samples, and their hardcoded ID
        /// </summary>
        private static readonly IReadOnlyCollection<(String Name, String FriendlyName, Guid Uid)> SamplesMap = new List<(String, String, Guid)>
        {
            ("HelloWorld.txt", "Hello world!", Guid.Parse("6B4C55E6-7009-48EC-96C5-C73552D9F257")),
            ("UnicodeValue.txt", LocalizationManager.GetResource("UnicodeValue"), Guid.Parse("10768D40-5E3D-4787-9CB8-2A0ABBE26EFC")),
            ("UnicodeSum.txt", LocalizationManager.GetResource("UnicodeSum"), Guid.Parse("78BAA70A-0DAF-4BB6-B09A-CDA9537D2FFF")),
            ("Sum.txt", LocalizationManager.GetResource("Sum"),  Guid.Parse("0441153F-E40A-4AEC-8373-8A552697778B")),
            ("HeaderComments.txt", LocalizationManager.GetResource("HeaderComments"),  Guid.Parse("63156CB7-1BD1-46EA-A705-AC2ADD4A5F11")),
            ("ExecuteIfZero.txt", "if (x == 0) then { }",  Guid.Parse("6DABC8A8-E32C-49A1-A348-CF836FEF276D"))
        };

        /// <summary>
        /// Refreshes the list of local code samples
        /// </summary>
        private async Task RefreshCodeSamplesAsync()
        {
            // Get the samples folder and iterate over the expected samples
            await EnsureDatabaseConnectionAsync();
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(SampleFilesPath);
            foreach ((String name, String friendly, Guid uid) in SamplesMap)
            {
                // Get the source code file and look for an existing copy in the local database
                StorageFile file = await folder.GetFileAsync(name);
                String sUid = uid.ToString();
                SourceCode row = await DatabaseConnection.Table<SourceCode>().Where(entry => entry.Uid == sUid).FirstOrDefaultAsync();
                if (row == null)
                {
                    // The code was missing, create a local copy in the database
                    String code = await FileIO.ReadTextAsync(file);
                    row = new SourceCode
                    {
                        Uid = uid.ToString(),
                        Title = friendly,
                        Code = code,
                        CreatedTime = DateTime.Now,
                        ModifiedTime = DateTime.Now
                    };
                    await DatabaseConnection.InsertAsync(row);
                }
            }
        }

        #endregion

        #region Public APIs

        /// <summary>
        /// Loads the saved source codes from the app database
        /// </summary>
        public async Task<IList<(SavedSourceCodeType Type, IList<SourceCode> Items)>> LoadSavedCodesAsync()
        {
            // Query the codes from the database
            await EnsureDatabaseConnectionAsync();
            await RefreshCodeSamplesAsync();
            List<SourceCode> codes = await DatabaseConnection.Table<SourceCode>().ToListAsync();

            // Populate the three categories
            List<SourceCode>
                samples = new List<SourceCode>(),
                favorites = new List<SourceCode>(),
                original = new List<SourceCode>();
            foreach (SourceCode code in codes)
            {
                if (SamplesMap.Any(sample => sample.Uid.Equals(Guid.Parse(code.Uid))))
                {
                    samples.AddSorted(code, entry => SamplesMap.IndexOf(sample => sample.Uid.Equals(Guid.Parse(entry.Uid))));
                }
                else if (code.Favorited) favorites.AddSorted(code, entry => entry.Title);
                else original.AddSorted(code, entry => entry.Title);
            }

            // Aggregate and return the results
            return new(SavedSourceCodeType, IList<SourceCode>)[]
            {
                (SavedSourceCodeType.Favorite, favorites.ToArray()),
                (SavedSourceCodeType.Original, original.ToArray()),
                (SavedSourceCodeType.Sample, samples.ToArray())
            };
        }

        /// <summary>
        /// Saves a new source code in the app local database
        /// </summary>
        /// <param name="title">The title of the source code to save</param>
        /// <param name="code">The code to save</param>
        public async Task<bool> SaveCodeAsync([NotNull] String title, [NotNull] String code)
        {
            await EnsureDatabaseConnectionAsync();
            if (!await CheckExistingName(title)) return false;
            SourceCode entry = new SourceCode
            {
                Uid = Guid.NewGuid().ToString(),
                Code = code,
                Title = title,
                CreatedTime = DateTime.Now,
                ModifiedTime = DateTime.Now
            };
            await DatabaseConnection.InsertAsync(entry);
            return true;
        }

        /// <summary>
        /// Saves the changes to a source code
        /// </summary>
        /// <param name="code">The source code that's being edited</param>
        /// <param name="text">The updated text to save</param>
        public async Task SaveCodeAsync([NotNull] SourceCode code, [NotNull] String text)
        {
            await EnsureDatabaseConnectionAsync();
            code.Code = text;
            code.ModifiedTime = DateTime.Now;
            await DatabaseConnection.UpdateAsync(code);
        }

        /// <summary>
        /// Checks whether or not there is a saved source code with the same title
        /// </summary>
        /// <param name="name">The title to check</param>
        public async Task<bool> CheckExistingName([NotNull] String name)
        {
            await EnsureDatabaseConnectionAsync();
            return await DatabaseConnection.Table<SourceCode>().Where(row => row.Title == name).FirstOrDefaultAsync() == null;
        }

        /// <summary>
        /// Renames a selected source code saved in the app
        /// </summary>
        /// <param name="code">The source code to rename</param>
        /// <param name="name">The new name for the saved code</param>
        public async Task RenameCodeAsync([NotNull] SourceCode code, [NotNull] String name)
        {
            await EnsureDatabaseConnectionAsync();
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
            await DatabaseConnection.DeleteAsync(code);
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

        #endregion
    }
}
