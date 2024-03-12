//using MongoDB.Bson;
//using MongoDB.Driver;
//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Text;
//using System.Threading.Tasks;

//namespace SocketMutiplayerGameServer.Tool
//{
//    public class MongoHelper
//    {
//        #region 配置信息

//        private static string connectionStr = string.Empty;
//        private static string ip = "127.0.0.1", port = "27017";
//        private static string dbName = "ServerGame";

//        public static string ConnectionStr
//        {
//            get
//            {
//                if (string.IsNullOrEmpty(connectionStr))
//                {
//                    connectionStr = "mongodb://" + ip + ":" + port;
//                }
//                return connectionStr;
//            }
//            set
//            {
//                connectionStr = value;
//            }
//        }
//        #endregion

//        IMongoDatabase db;

//        public MongoHelper()
//        {
//            var client = new MongoClient(ConnectionStr);
//            db = client.GetDatabase(dbName);
//        }

//        public void Close()
//        {
//            if (db != null)
//            {
//                db = null;
//            }
//        }

//        /// <summary>
//        /// 添加数据
//        /// </summary>
//        /// <param name="tableName"></param>
//        /// <param name="bson"></param>

//        public void Insert(string tableName, BsonDocument bson)
//        {
//            var col = db.GetCollection<BsonDocument>(tableName);
//            col.InsertOne(bson);
//        }

//        /// <summary>
//        /// 插入多条数据
//        /// </summary>
//        /// <param name="tableName"></param>
//        /// <param name="list"></param>
//        /// <returns></returns>
//        public async Task InsertManyAsync(string tableName, IEnumerable<BsonDocument> list)
//        {
//            var col = db.GetCollection<BsonDocument>(tableName);
//            await col.InsertManyAsync(list);
//        }

//        /// <summary>
//        /// 获取符合条件数据数量
//        /// </summary>
//        /// <param name="tableName"></param>
//        /// <param name="filter"></param>
//        /// <returns></returns>
//        public long Count(string tableName, FilterDefinition<BsonDocument> filter)
//        {
//            var col = db.GetCollection<BsonDocument>(tableName);
//            var count = col.CountDocuments(filter);
//            return count;
//        }

//        /// <summary>
//        /// 更新数据
//        /// </summary>
//        /// <param name="tableName"></param>
//        /// <param name="filter"></param>
//        /// <param name="update"></param>
//        /// <returns></returns>

//        public BsonDocument Update(string tableName, FilterDefinition<BsonDocument> filter, BsonDocument update)
//        {
//            var col = db.GetCollection<BsonDocument>(tableName);
//            var result = col.FindOneAndUpdate(filter, update);
//            return result;
//        }



//        /// <summary>
//        /// 删除数据
//        /// </summary>
//        /// <param name="tableName"></param>
//        /// <param name="filter"></param>
//        /// <returns></returns>

//        public BsonDocument Delete(string tableName, FilterDefinition<BsonDocument> filter)
//        {
//            var col = db.GetCollection<BsonDocument>(tableName);
//            var result = col.FindOneAndDelete(filter);
//            return result;
//        }



//        /// <summary>
//        /// 查询数据
//        /// </summary>
//        /// <param name="tableName"></param>
//        /// <param name="filter"></param>
//        /// <returns></returns>

//        public IAsyncCursor<BsonDocument> FindAll(string tableName, FilterDefinition<BsonDocument> filter)
//        {
//            var col = db.GetCollection<BsonDocument>(tableName);
//            var cursor = col.Find(filter).ToCursor();
//            return cursor;
//        }

//        /// <summary>
//        /// 查询单条数据
//        /// </summary>
//        /// <param name="tableName"></param>
//        /// <param name="filter"></param>
//        /// <returns></returns>
//        public BsonDocument FindOne(string tableName, FilterDefinition<BsonDocument> filter, SortDefinition<BsonDocument> sort = null)
//        {
//            var col = db.GetCollection<BsonDocument>(tableName);
//            var result = col.Find(filter);
//            BsonDocument bson;
//            if (sort != null)
//            {
//                bson = result.Sort(sort).FirstOrDefault();
//            }
//            else
//            {
//                bson = result.FirstOrDefault();
//            }
//            return bson;
//        }
//    }
//}
