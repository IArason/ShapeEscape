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
    
    conn = mysql_connect.connect()

    try:
        with conn.cursor() as cur:
            #Check if ID is already present
            sql = "select PlayerName from Player where PlayerID=%s"
            cur.execute(sql, event["PlayerID"])
            result = cur.fetchone()
            
            if(result == None):
                print("None")
                returnVal = { "Message" : "Invalid ID"}
            else:
                print(result["PlayerName"])
                returnVal = { "PlayerName" : result["PlayerName"], "Message" : "Success" }
         
    except Exception, err:
        returnVal = { "Message" : "Exception: " + str(err) }
        
    finally:
        conn.close()
        return returnVal
    
def validate(event):
    return (event.has_key("PlayerID"))
    
'''
def main():
    print(handler({
    "PlayerID": "us-east-1:361638c8-1cac-413c-9f2e-02cd4a5e499e"
    }, None))
    
if True:
    main()
'''