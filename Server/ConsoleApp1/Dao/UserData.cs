//using MongoDB.Bson;
//using MongoDB.Driver;
using SocketGameProtocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocketMutiplayerGameServer.Dao
{
    class UserData : BaseData
    {
        string tableName = "User";

        public bool Logon(MainPack pack)
        {
            string userName = pack.LoginPack.Username;
            string password = pack.LoginPack.Password;
            if(String.IsNullOrWhiteSpace(userName) || String.IsNullOrWhiteSpace(password))
            {
                return false;
            }
            //BsonDocument bson = new BsonDocument();
            //bson.Set("userName",userName);
            //bson.Set("password",password);
            //mongoHelper.Insert(tableName,bson);
            return true;
        }

        public bool Login(MainPack pack)
        {
            string userName = pack.LoginPack.Username;
            string password = pack.LoginPack.Password;
            if (String.IsNullOrWhiteSpace(userName) || String.IsNullOrWhiteSpace(password))
            {
                return false;
            }
            //var builder = Builders<BsonDocument>.Filter;
            //var filter = builder.Eq("userName", userName) & builder.Eq("password", password);
            //var result = mongoHelper.FindOne(tableName,filter);

            return true;// result != null;
        }

        public void Close()
        {
            //mongoHelper.Close();
        }
    }
}
