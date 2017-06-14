using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Brainf_ck_sharp_UWP.DataModels.SQLite;
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

        /// <summary>
        /// Gets a collection of names for the code samples, and their hardcoded ID
        /// </summary>
        private static readonly IReadOnlyCollection<(String, Guid)> SamplesMap = new List<(String Name, Guid Uid)>
        {
            ("HelloWorld.txt", Guid.Parse("6B4C55E6-7009-48EC-96C5-C73552D9F257")),
            ("UnicodeValue.txt", Guid.Parse("10768D40-5E3D-4787-9CB8-2A0ABBE26EFC")),
            ("UnicodeSum.txt", Guid.Parse("78BAA70A-0DAF-4BB6-B09A-CDA9537D2FFF")),
            ("Sum.txt", Guid.Parse("0441153F-E40A-4AEC-8373-8A552697778B"))
        };

        /// <summary>
        /// Refreshes the list of local code samples
        /// </summary>
        public async Task RefreshCodeSamplesAsync()
        {
            // Get the samples folder and iterate over the expected samples
            await EnsureDatabaseConnectionAsync();
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(SampleFilesPath);
            foreach ((String name, Guid uid) in SamplesMap)
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
                        Code = code,
                        CreatedTime = DateTime.Now,
                        ModifiedTime = DateTime.Now
                    };
                    await DatabaseConnection.InsertAsync(row);
                }
            }
        }

        public async Task<IReadOnlyList<SourceCode>> LoadSavedCodesAsync()
        {
            await EnsureDatabaseConnectionAsync();
            throw new NotImplementedException();
        }

        public async Task<bool> SaveCodeAsync([NotNull] SourceCode code, [NotNull] String text)
        {
            await EnsureDatabaseConnectionAsync();
            throw new NotImplementedException();
        }


    }
}
