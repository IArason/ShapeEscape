import sys
import logging
import rds_config
import mysql_connect
import pymysql
import json

"""
Sets the player's name. If a name is already associated with this ID,
update the name instead.
"""

def handler(event, context):
    """
    This function fetches content from mysql RDS instance
    """
    if(not validate(event)):
        return { "Message" : "Invalid data"}
    
    conn = mysql_connect.connect()

    try:
        with conn.cursor() as cur:
        
            #Check if name is already taken
            sql = "select * from `Player` where `PlayerName`=%s limit 1"
            cur.execute(sql, (event["PlayerName"],))
            result = cur.fetchone()
            if(result != None and result["PlayerID"] != event["PlayerID"]):
                returnVal = { "Message" : "Name already in use" }
            else:
                returnVal = { "Message" : "Success" }
                #Check if player already has a name
                sql = "select 1 from `Player` where `PlayerID`=%s limit 1"
                cur.execute(sql, (event["PlayerID"],))
                result = cur.fetchone()
                if(result != None): #If player does have a name
                    sql = "update `Player` set `PlayerName`=%s where `PlayerID`=%s"
                    cur.execute(sql, (event["PlayerName"], event["PlayerID"]))
                else:
                    sql = "insert into Player (PlayerID, PlayerName) values (%s, %s)"
                    cur.execute(sql, (event["PlayerID"], event["PlayerName"]))
            
        conn.commit()
            
    except Exception, err:
        returnVal = { "Message" : "Exception: " + str(err) }
    finally:
        conn.close()
        return returnVal
    
def validate(event):
    return (event.has_key("PlayerID") and event.has_key("PlayerName"))

'''    
def main():
    print(handler({
    "PlayerID": "TestID",
    "PlayerName" : "MyTestName"
    }, None))
    
if True:
    main()
'''