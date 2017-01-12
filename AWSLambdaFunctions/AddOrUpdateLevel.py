import sys
import rds_config
import mysql_connect
import pymysql
import json


def handler(event, context):
    """
    This function fetches content from mysql RDS instance
    """
    if(not validate(event)):
        return { "Message" : "Invalid data" }
        sys.exit()
    
    returnVal = {"Message":""}
    conn = mysql_connect.connect()
    
    LevelID = "None"
     
    try:
        with conn.cursor() as cur:
            
            # If event includes ID, save using ID and overwrite name
            if(event.has_key("LevelID")):
                sql = "select * from `Level` where `LevelID`=%s limit 1"
                cur.execute(sql, (event["LevelID"],))
                result = cur.fetchone()
                
                # Result is (LevelID, PlayerID, LevelName)
                if(result == None or result["PlayerID"] != event["PlayerID"]):
                    returnVal = { "Message" : "Invalid ID" }
                    sys.exit()
                   
                levelName = result["LevelName"]
                if(event.has_key("LevelName")):
                    levelName = event["LevelName"]
                
                sql = "update `Level` set `LevelName`=%s, `UploadTime`=NOW() where `LevelID`=%s"
                cur.execute(sql, (levelName, result["LevelID"]))
                
                sql = "update `LevelData` set `LevelData`=%s where `LevelID`=%s"
                cur.execute(sql, (event["LevelData"], result["LevelID"]))
                levelID = result["LevelID"]
                
            else:
                # If not, use name to add new level to the database
                # Check if name already exists
                sql = "select * from Level where LevelName=%s and PlayerID=%s limit 1"
                cur.execute(sql, (event["LevelName"], event["PlayerID"]))
                result = cur.fetchone() #Returns None or ID
                if(result == None): #Add the level
                    sql = "insert into `Level` (PlayerID, LevelName, UploadTime) values (%s, %s, NOW())"
                    cur.execute(sql, (event["PlayerID"], event["LevelName"]))
                    sql = "SELECT LAST_INSERT_ID()"
                    cur.execute(sql)
                    levelID = cur.fetchone().itervalues().next(); #grabs the ID of the inserted level
                    sql = "insert into `LevelData` (LevelID, levelData) values (%s, %s)"
                    cur.execute(sql, (levelID, event["LevelData"]))
                else:
                    print(result)
                    returnVal = { "LevelID" : result["LevelID"],
                        "Message" : "Level name already exists" } #Return the level ID
                    sys.exit()
                
            conn.commit()
            returnVal = { "LevelID" : levelID, 
                        "Message" : "Success" }
            
    except Exception, err:
        returnVal = { "Message" : returnVal["Message"] + " | Exception:" + str(err) }
        sys.exit()
        
    finally:
        conn.close()
        return returnVal
    
def validate(event):
    return (event.has_key("PlayerID") and 
    event.has_key("LevelName") and
    event.has_key("LevelData"))

'''
def main():
    print(handler({
    "PlayerID": "us-east-1:361638c8-1cac-413c-9f2e-02cd4a5e499e",
    "LevelName":"TestLevel",
    "LevelID":"33",
    "LevelData":"{}"
    }, None))
    print(handler({
    "PlayerID": "us-east-1:361638c8-1cac-413c-9f2e-02cd4a5e499e",
    "LevelName":"TestLevel2",
    "LevelData":"{}"
    }, None))
    
if True:
    main()
'''