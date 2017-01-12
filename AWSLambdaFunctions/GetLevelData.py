import sys
import rds_config
import mysql_connect
import pymysql
import json


def handler(event, context):

    if(not validate(event)):
        return { "Message" : "Invalid data" }
        
    conn = mysql_connect.connect()
        
    try:
        with conn.cursor() as cur:
        
            sql = "SELECT LevelData FROM LevelData WHERE LevelID=%s"
            cur.execute(sql, (event["LevelID"],))
            result = cur.fetchone()
            
            if(result == None):
                returnVal = {"Message" : "Level ID does not exist"}
            else:
                returnVal = {"Message" : "Success", "LevelData" : result["LevelData"]}
                
            sql = "UPDATE Level SET PlaythroughCount = PlaythroughCount + 1 WHERE LevelID=%s"
            
            cur.execute(sql, (event["LevelID"]))
            
        conn.commit()
            
    except Exception, err:
        returnVal = { "Message" : str(err) }
        sys.exit()
        
    finally:
        conn.close()
        return returnVal
        
def validate(event):
    return (event.has_key("LevelID"))