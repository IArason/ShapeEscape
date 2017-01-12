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
        return { "Message" : "Invalid data" }
        
    returnVal = {"Message":""}
    conn = mysql_connect.connect()

    try:
        with conn.cursor() as cur:
        
            link = int(str(hash(str(event["LevelID"])))[-7:])
            print(link)
            
            # Verify that level exists
            sql = "SELECT LevelID FROM Level WHERE LevelID=%s"
            cur.execute(sql, (event["LevelID"],))
            
            if(cur.fetchone() == None):
                returnVal = {"Message":"Invalid ID"}
                sys.exit()
            
            sql = "SELECT ShareableLink FROM ShareableLink WHERE LevelID=%s"
            cur.execute(sql, (event["LevelID"],))
            result = cur.fetchone()
            
            if(result == None):
                sql = "INSERT INTO ShareableLink (ShareableLink, LevelID) VALUES (%s, %s)"
                cur.execute(sql, (link, event["LevelID"]))
                conn.commit()
                result = {"ShareableLink":link}
                
            cur.close()
            returnVal = {"Message":"Success", "Link":result["ShareableLink"]}
            
    except Exception, err:
        returnVal = { "Message" : returnVal["Message"] + " | Exception: " + str(err) }
        
    finally:
        conn.close()
        return returnVal
def validate(event):
    return (event.has_key("LevelID"))
    
def main():
    print(handler({
    "LevelID": "45"
    }, None))
    
if True:
    main()