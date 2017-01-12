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
        return {"Messages" : [{"Message" : "Invalid Data"}]}
        sys.exit()
        
    conn = mysql_connect.connect()
        
    try:
        with conn.cursor() as cur:
        
            
            # Check if ID or Name exist
            if(not event.has_key("PlayerID")):
                sql = "SELECT PlayerID FROM Player WHERE PlayerName=%s LIMIT 1"
                cur.execute(sql, event["PlayerName"])
                result = cur.fetchone()
                if(result == None):
                    returnVal = {"Messages" : [{"Message" : "Player does not exist"}]}
                    sys.exit()
                PlayerID = result["PlayerID"]
            else:
                PlayerID = event["PlayerID"]
            
            # Returns all levels made by PlayerID in the format of
            # LevelID, LevelName, PlayerName, AverageRating
            sql = ("SELECT LevelID, LevelName, PlayerName, PlaythroughCount " 
                    "FROM Level "
                    "LEFT JOIN Player ON Level.PlayerID=Player.PlayerID "
                    "WHERE Level.PlayerID=%s")
            
            cur.execute(sql, (PlayerID,))
            result = cur.fetchall()
            
            returnVal = {"Messages" : [{"Message" : "Success"}], "Levels" : result}
            
        conn.commit()
            
    except Exception, err:
        returnVal =  {"Messages" : [{"Message" : "Exception: " + str(err)}]}
    finally:
        conn.close()
        return returnVal
        
def validate(event):
    return (event.has_key("PlayerID") or 
    event.has_key("PlayerName"))
'''
def main():
    print(handler({
    "PlayerID": "TestID"
    }, None))
    
if True:
    main()
'''