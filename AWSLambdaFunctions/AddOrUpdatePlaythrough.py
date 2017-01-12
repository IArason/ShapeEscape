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

conn = mysql_connect.connect()

def handler(event, context):
    """
    This function fetches content from mysql RDS instance
    """
    if(not validate(event)):
        return "Failure"
        sys.exit()
    
    try:
        with conn.cursor() as cur:
            sql = "select 1 `PlayerName` from `Player` where `PlayerID`=%s"
            cur.execute(sql, (event["PlayerID"],))
            
            result = cur.fetchone()
            
            if(result != None):
                sql = "update `Player` set `PlayerName`=%s where `PlayerID`=%s"
                cur.execute(sql, (event["PlayerName"], event["PlayerID"]))
            else:
                sql = "insert into Player (PlayerID, PlayerName) values (%s, %s)"
                cur.execute(sql, (event["PlayerID"], event["PlayerName"]))
            
            conn.commit()
            
        return "Success"
    except Exception, err:
        return "Failure"
        sys.exit()
    
def validate(event):
    return (event.has_key("PlayerID") and event.has_key("PlayerName"))