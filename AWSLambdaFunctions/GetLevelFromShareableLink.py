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
    
    returnVal = {"Message":""}
    conn = mysql_connect.connect()

    try:
        with conn.cursor() as cur:
            sql = ("SELECT ShareableLink.LevelID, "
                    "LevelName, "
                    "( SELECT PlayerName "
                    "FROM Player "
                    "WHERE Player.PlayerID = Level.PlayerID) AS PlayerName "
                    "FROM Level "
                    "LEFT JOIN ShareableLink ON ShareableLink.LevelID=Level.LevelID "
                    "WHERE ShareableLink.ShareableLink=%s")
                    
            cur.execute(sql, (event["ShareableLink"]))
            result = cur.fetchone()
            print(result);
            
            if(result == None):
                returnVal = {"Message":"Invalid link"}
            else:
                returnVal = {"Messages" : [{"Message" : "Success"}], "Levels" : result}
            
    except Exception, err:
        returnVal = { "Message" : returnVal["Message"] + str(err) }
        
    finally:
        conn.close()
        return returnVal
    
def validate(event):
    return (event.has_key("ShareableLink"))
    
def main():
    print(handler({
    "ShareableLink": "1833215763"
    }, None))
    
if True:
    main()