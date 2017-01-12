import sys
import logging
import rds_config
import mysql_connect
import pymysql
import json


def handler(event, context):
    """
    This function fetches content from mysql RDS instance
    """
    if(not validate(event)):
        return { "Message" : "Invalid data"}
        
    conn = mysql_connect.connect()
    
    try:
        with conn.cursor() as cur:
        
            #Check if level exists
            sql = "SELECT `PlayerID` FROM `Level` WHERE `LevelID`=%s LIMIT 1"
            cur.execute(sql, (event["LevelID"],))
            result = cur.fetchone()
            
            if(result == None):
                returnVal = {"Message":"Level does not exist"}
                sys.exit()
            
            if(result["PlayerID"] != event["PlayerID"]):
                returnVal = { "Message" : "Invalid ID"}
                sys.exit()
            
            print("Test")
            sql = "DELETE FROM `LevelData` WHERE `LevelID`=%s LIMIT 1"
            sql = "DELETE FROM `Level` WHERE `LevelID`=%s LIMIT 1"
            cur.execute(sql, (event["LevelID"],))
            
        conn.commit()
            
        returnVal = { "Message" : "Success"}
        
    except Exception, err:
        returnVal = { "Message" : returnVal["Message"] + " | Exception:" + str(err) }
        sys.exit()
        
    finally:
        conn.close()
        return returnVal;
        
def validate(event):
    return (event.has_key("PlayerID") and event.has_key("LevelID"))
'''
def main():
    print(handler({
    "LevelID" : "3",
    "PlayerID" : "TestID"
    }, None))
    
if True:
    main()
'''