using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NPOI.HSSF.Record;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Agile.BaseLib.Helpers
{
    public class RedisHelper
    {
        //锁
        private static readonly object Locker = new object();

        private ConnectionMultiplexer redisMultiplexer;

        IDatabase db = null;

        private static RedisHelper _redisClient = null;
       
        public static RedisHelper redisClient
        {
            get
            {
                if (_redisClient == null)
                {
                    lock (Locker)
                    {
                        if (_redisClient == null)
                        {
                            _redisClient = new RedisHelper();

                        }
                    }
                }
                return _redisClient;
            }
        }

        public void InitConnect(IConfiguration Configuration)
        {
            try
            {          
                var RedisConnection = Configuration.GetSection("Redis:Default");
                var ConnectionStr = RedisConnection.GetSection("Connection").Value;
                redisMultiplexer = ConnectionMultiplexer.Connect(ConnectionStr);
                db = redisMultiplexer.GetDatabase();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                redisMultiplexer = null;
                db = null;
            }
        }
        /// <summary>
        /// 保存单个key value
        /// </summary>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        public bool SetStringKey(string key, string value, TimeSpan? expiry = default(TimeSpan?))
        {
            return db.StringSet(key, value, expiry);
        }

        /// <summary>
        /// 获取单个key的值
        /// </summary>
        public RedisValue GetStringKey(string key)
        {
            return db.StringGet(key);
        }


        /// <summary>
        /// 获取一个key的对象
        /// </summary>
        public T GetStringKey<T>(string key)
        {
            if (db == null)
            {
                return default;
            }
            var value = db.StringGet(key);
            if (value.IsNullOrEmpty)
            {
                return default;
            }
            return JsonConvert.DeserializeObject<T>(value);
        }

        /// <summary>
        /// 保存一个对象
        /// </summary>
        /// <param name="obj"></param>
        public bool SetStringKey<T>(string key, T obj, TimeSpan? expiry = default(TimeSpan?))
        {
            if (db == null)
            {
                return false;
            }
            string json = JsonConvert.SerializeObject(obj);
            return db.StringSet(key, json, expiry);
        }
    }
}
