import mysql.connector
import datetime

def_string = ("CREATE TABLE IF NOT EXISTS users (UID VARCHAR(12) NOT NULL PRIMARY KEY, name VARCHAR(25) NOT NULL "
              "UNIQUE, pass VARCHAR(25) NOT NULL, t_id VARCHAR(255), bal INT DEFAULT(0), liv INT DEFAULT(0), "
              "av LONGBLOB DEFAULT(null));")


class DataBase:
    def __init__(self, host, user, password, database):
        self.connection = None
        self.host = host
        self.user = user
        self.password = password
        self.database = database
        self.err = ErrorLog("error.txt")

    def open(self):
        try:
            self.connection = mysql.connector.connect(
                host=self.host,
                user=self.user,
                password=self.password,
                database=self.database
            )
            return True
        except Exception as err:
            self.err.write(err)
            return False

    def close(self):
        if self.connection is not None:
            self.connection.close()
            return True
        else:
            return False

    def query(self, query):
        if not self.open():
            print("Failed to open database connection.")
            return
        cursor = self.connection.cursor()
        cursor.execute(query)
        cursor.close()
        self.close()

    def select(self, table, columns=None, where=None):
        try:
            self.open()
            cursor = self.connection.cursor()
            if where is not None:
                cursor.execute(
                    f"SELECT {columns if columns is not None else '*'} FROM " + table + " WHERE " + where + ";")
            else:
                cursor.execute(f"SELECT {columns if columns is not None else '*'} FROM " + table + ";")
            result = cursor.fetchall()
            cursor.close()
            self.close()
            return result
        except Exception as err:
            self.err.write(err)

    def insert(self, table, columns, values):
        try:
            self.open()
            cursor = self.connection.cursor()
            cursor.execute(f"INSERT INTO {table} ({columns}) VALUES ({values});")
            self.connection.commit()
            cursor.close()
            self.close()
        except Exception as err:
            self.err.write(err)

    def update(self, table, columns, values, where):
        try:
            self.open()
            cursor = self.connection.cursor()
            col_str = ""
            for i in range(len(columns)):
                col_str += f"`{columns[i]}` = '{values[i]}', "
            col_str = col_str.rstrip(", ")  # Remove the trailing comma and space
            query = f"UPDATE `{table}` SET {col_str} WHERE {where}"
            cursor.execute(query)
            self.connection.commit()
            cursor.close()
            self.close()
        except Exception as err:
            print(err)

    def update_blob(self, table, column, blob_value, where):
        try:
            self.open()
            cursor = self.connection.cursor()
            query = f"UPDATE `{table}` SET `{column}` = %s WHERE {where}"
            cursor.execute(query, (blob_value,))
            self.connection.commit()
            cursor.close()
            self.close()
        except Exception as err:
            print(err)

    def delete(self, table, where):
        self.open()
        cursor = self.connection.cursor()
        cursor.execute(f"DELETE FROM {table} WHERE {where};")
        cursor.close()
        self.close()

    def __del__(self):
        self.connection.close()


class ErrorLog:
    def __init__(self, path):
        self.path = path

    def write(self, text: Exception):
        current_time = datetime.datetime.now().strftime("%d.%m.%Y %H:%M")
        with open(self.path, "a") as file:
            file.write(f"{current_time} ===================\n{text}\n")
