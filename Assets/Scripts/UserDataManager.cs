﻿using Firebase.Storage;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Yashlan.data;

namespace Yashlan.manage
{
    public class UserDataManager : MonoBehaviour
    {
        private const string PROGRESS_KEY = "Progress";
        private const string USER_PROPERTIES_NAME = "GOLD";

        public static UserProgressData Progress = new UserProgressData();

        public static void LoadFromLocal()
        {
            // Cek apakah ada data yang tersimpan sebagai PROGRESS_KEY
            if (!PlayerPrefs.HasKey(PROGRESS_KEY))
            {
                // Jika tidak ada, maka simpan data baru
                // dan upload ke Cloud
                Save(true);
            }
            else
            {
                // Jika ada, maka timpa progress dengan yang sebelumnya
                string json = PlayerPrefs.GetString(PROGRESS_KEY);
                Progress = JsonUtility.FromJson<UserProgressData>(json);
            }
        }

        public static IEnumerator LoadFromCloud(System.Action onComplete)
        {
            StorageReference targetStorage = GetTargetCloudStorage();

            bool isCompleted = false;
            bool isSuccessfull = false;
            const long maxAllowedSize = 1024 * 1024; // Sama dengan 1 MB
            targetStorage.GetBytesAsync(maxAllowedSize).ContinueWith((Task<byte[]> task) =>
            {
                if (!task.IsFaulted)
                {
                    string json = Encoding.Default.GetString(task.Result);
                    Progress = JsonUtility.FromJson<UserProgressData>(json);
                    isSuccessfull = true;
                }

                isCompleted = true;
            });

            while (!isCompleted)
            {
                yield return null;
            }

            if (isSuccessfull)
                // Jika sukses mendownload, maka simpan data hasil download
                Save();
            else
                // Jika tidak ada data di cloud, maka load data dari local
                LoadFromLocal();

            onComplete?.Invoke();
        }

        public static void Save(bool uploadToCloud = false)
        {
            string json = JsonUtility.ToJson(Progress);
            PlayerPrefs.SetString(PROGRESS_KEY, json);

            if (uploadToCloud)
            {
                AnalyticsManager.SetUserProperties(USER_PROPERTIES_NAME, Progress.Gold.ToString());

                byte[] data = Encoding.Default.GetBytes(json);
                StorageReference targetStorage = GetTargetCloudStorage();

                targetStorage.PutBytesAsync(data);
            }
        }


        private static StorageReference GetTargetCloudStorage()
        {
            // Gunakan Device ID sebagai nama file yang akan disimpan di cloud
            string deviceID = SystemInfo.deviceUniqueIdentifier;
            FirebaseStorage storage = FirebaseStorage.DefaultInstance;
            return storage.GetReferenceFromUrl($"{storage.RootReference}/{deviceID}");
        }

        public static bool HasResources(int index) => index + 1 <= Progress.ResourcesLevels.Count;
    }
}