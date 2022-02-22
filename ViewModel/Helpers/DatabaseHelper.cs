using MyNotes.Model;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MyNotes.ViewModel.Helpers
{
    public class DatabaseHelper
    {
        private static string dbFile = Path.Combine(Environment.CurrentDirectory, "notesDB.db3");
        private static string dbPath = "https://mynotes-ca099-default-rtdb.firebaseio.com/";
        public async static Task<bool> Insert<T>(T item)
        {
            //    bool result = false;
            //    using (SQLiteConnection conn =new SQLiteConnection(dbFile))
            //    {
            //        conn.CreateTable<T>();
            //        int rows= conn.Insert(item);
            //        if(rows>0)
            //        {
            //            result = true;
            //        }
            //    }
            //    return result;

            string jsonBody = JsonConvert.SerializeObject(item);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {
                var result = await client.PostAsync($"{dbPath}{item.GetType().Name.ToLower()}.json", content);

                if (result.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static async Task<bool> Update<T>(T item) where T : HasId
        {          
            string jsonBody = JsonConvert.SerializeObject(item);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {
                var result = await client.PatchAsync($"{dbPath}{item.GetType().Name.ToLower()}/{item.Id}.json", content);

                if (result.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static async Task<bool> Delete<T>(T item) where T : HasId
        {
            
            using (var client = new HttpClient())
            {
                
                var result = await client.DeleteAsync($"{dbPath}{item.GetType().Name.ToLower()}/{item.Id}.json");

                if (result.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static async Task<List<T>> Read<T>() where T : HasId
        {
            //List<T> items;

            //using (SQLiteConnection conn = new SQLiteConnection(dbFile))
            //{
            //    conn.CreateTable<T>();
            //    items = conn.Table<T>().ToList();
            //}

            //return items;

            using (var client = new HttpClient())
            {
                List<T> list = new List<T>();
                var result = await client.GetAsync($"{dbPath}{typeof(T).Name.ToLower()}.json");
                var jsonResult = await result.Content.ReadAsStringAsync();

                if (!result.IsSuccessStatusCode)
                {
                    return list;
                }
                var objects = JsonConvert.DeserializeObject<Dictionary<string, T>>(jsonResult);

                if (objects == null)
                {
                    return list;
                }
                foreach (var o in objects)
                {
                    o.Value.Id = o.Key;
                    list.Add(o.Value);
                }

                return list;

            }
        }
    }
}
