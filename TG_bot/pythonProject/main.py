import io
import os
import random
import string

import telebot
from PIL import Image
from telebot import types

from handler import DataBase, def_string, ErrorLog

bot = telebot.TeleBot('7146246279:AAF0pR-XCaLpYhtG3LKVs2CHjA_XQI7MG-4')
db = DataBase("127.0.0.1", "root", "root", "Nomadic")
#db = DataBase("20.52.101.204", "c0nstanta", "03062008@rjitdjqK", "Nomadic")
el = ErrorLog("error.txt")
admin = "@C0nstatinus"


def img_to_bytes(image):
    img = Image.open(image)
    byte_arr = io.BytesIO()
    if img.mode != 'RGB':
        img = img.convert('RGB')
    img.save(byte_arr, format='JPEG')
    blob = byte_arr.getvalue()
    return blob

def start_menu():
    markup = types.ReplyKeyboardMarkup(resize_keyboard=True)
    btn1 = types.KeyboardButton("Мой профиль")
    btn2 = types.KeyboardButton("Поддержка проекта")
    btn3 = types.KeyboardButton("Выйти из аккаунта")
    markup.row(btn1, btn2)
    markup.row(btn3)
    return markup


def profile_markup():
    markup = types.ReplyKeyboardMarkup(resize_keyboard=True)
    btn1 = types.KeyboardButton("Изменить имя")
    btn2 = types.KeyboardButton("Изменить пароль")
    btn3 = types.KeyboardButton("Изменить аватар")
    btn4 = types.KeyboardButton("Вернуться")
    markup.row(btn1, btn3, btn2)
    markup.row(btn4)
    return markup


def pre_start_markup():
    markup = types.ReplyKeyboardMarkup(resize_keyboard=True)
    btn1 = types.KeyboardButton("Зарегистрироваться")
    btn2 = types.KeyboardButton("Войти")
    markup.add(btn1, btn2)
    return markup


@bot.message_handler(commands=['start'])
def start_message(message):
    db.query(def_string)
    result = db.select("users", where=f"t_id = {message.from_user.id}")
    if not result:
        bot.send_message(message.chat.id, "Добро пожаловать в бота!", reply_markup=pre_start_markup())
        bot.register_next_step_handler(message, pre_main_menu)
    else:
        bot.send_message(message.chat.id, "Добро пожаловать!", reply_markup=start_menu())
        bot.register_next_step_handler(message, main_menu)


def main_menu(message):
    if message.text == "Мой профиль":
        result = db.select("users", "name, bal", f"t_id = {message.from_user.id}")
        bot.send_message(message.chat.id, f"Имя: {result[0][0]}\nБаланс: {result[0][1]}", reply_markup=profile_markup())
        bot.register_next_step_handler(message, profile_menu)
    elif message.text == "Поддержка проекта":
        bot.send_message(message.chat.id, "Поддержать проект вы можете через [kaspi](https://pay.kaspi.kz/pay/u3moldyc)", parse_mode='Markdown')
        bot.send_message(message.chat.id, "Спасибо за поддержку!")
        bot.send_message(message.chat.id, "Главное меню", reply_markup=start_menu())
        bot.register_next_step_handler(message, main_menu)
    elif message.text == "Выйти из аккаунта":
        db.update("users", ["t_id"], ["0"], f"t_id = {message.from_user.id}")
        bot.send_message(message.chat.id, "Вы успешно вышли из аккаунта!")
        start_message(message)


def profile_menu(message):
    if message.text == "Изменить имя":
        bot.send_message(message.chat.id, "Введите новое имя:", reply_markup=types.ReplyKeyboardRemove())
        bot.register_next_step_handler(message, change_name)
    elif message.text == "Изменить пароль":
        bot.send_message(message.chat.id, "Введите старый пароль:")
        bot.register_next_step_handler(message, change_pass)
    elif message.text == "Изменить аватар":
        bot.send_message(message.chat.id, "Отправьте новый аватар:")
        bot.register_next_step_handler(message, change_avatar)
    if message.text == "Вернуться" or message.text == "/start":
        bot.send_message(message.chat.id, "Главное меню", reply_markup=start_menu())
        bot.register_next_step_handler(message, main_menu)


def change_avatar(message):
    if message.photo is None and message.document is None:
        bot.send_message(message.chat.id, "Вы не отправили фотографию. Пожалуйста, попробуйте еще раз.",
                         reply_markup=profile_markup())
        bot.register_next_step_handler(message, profile_menu)
    else:
        if message.photo is not None:
            file_id = message.photo[-1].file_id
        else:
            if message.document.mime_type.startswith('image'):
                file_id = message.document.file_id
            else:
                bot.send_message(message.chat.id, "Отправленный файл не является изображением. Пожалуйста, попробуйте "
                                                  "еще раз.", reply_markup=profile_markup())
                bot.register_next_step_handler(message, profile_menu)
                return

        file_info = bot.get_file(file_id)
        file = bot.download_file(file_info.file_path)
        with open(f"avatars/{message.from_user.id}.jpg", "wb") as f:
            f.write(file)
        binary_data = img_to_bytes(f"avatars/{message.from_user.id}.jpg")
        db.update_blob('users', 'av', binary_data, f"t_id = {message.from_user.id}")
        bot.send_message(message.chat.id, "Аватар успешно изменен!", reply_markup=profile_markup())
        os.remove(f"avatars/{message.from_user.id}.jpg")
        bot.register_next_step_handler(message, profile_menu)


def change_name(message):
    db.update("users", ["name"], [f'{message.text}'], f"`t_id` = '{message.from_user.id}'")
    bot.send_message(message.chat.id, "Имя успешно изменено!", reply_markup=profile_markup())
    bot.register_next_step_handler(message, profile_menu)


def change_pass(message):
    result = db.select("users", "pass", f"`t_id` = '{message.from_user.id}'")
    if message.text == result[0][0]:
        bot.send_message(message.chat.id, "Введите новый пароль:")
        bot.register_next_step_handler(message, change_pass2)
    elif "/start" == message.text:
        bot.send_message(message.chat.id, "Главное меню", reply_markup=start_menu())
        bot.register_next_step_handler(message, main_menu)
    else:
        bot.send_message(message.chat.id,
                         f"Неверный пароль, попробуйте еще раз \n Или свяжитесь с администратором {admin}",
                         reply_markup=profile_markup())
        bot.register_next_step_handler(message, profile_menu)


def change_pass2(message):
    db.update("users", ["pass"], [f'{message.text}'], f"t_id = {message.from_user.id}")
    bot.send_message(message.chat.id, "Пароль успешно изменен!", reply_markup=profile_markup())
    bot.register_next_step_handler(message, profile_menu)


def pre_main_menu(message):
    if message.text == "Зарегистрироваться":
        bot.send_message(message.chat.id, "Введите имя:", reply_markup=types.ReplyKeyboardRemove())
        bot.register_next_step_handler(message, reg_name)
    elif "/start" == message.text:
        bot.send_message(message.chat.id, "Главное меню", reply_markup=pre_start_markup())
        bot.register_next_step_handler(message, main_menu)
    elif message.text == "Войти":
        bot.send_message(message.chat.id, "Введите имя:", reply_markup=types.ReplyKeyboardRemove())
        bot.register_next_step_handler(message, login_name)


def reg_name(message):
    bot.send_message(message.chat.id, "Введите пароль")
    bot.register_next_step_handler(message, reg_pass, message.text)


def reg_pass(message, login):
    if "/start" == message.text:
        bot.send_message(message.chat.id, "Главное меню", reply_markup=pre_start_markup())
        bot.register_next_step_handler(message, main_menu)
    try:
        result = db.select("users", "UID")
        uid = generate_unique_uid(result)
        db.insert("users", "name, pass, t_id, UID", f"'{login}', '{message.text}', '{message.from_user.id}', '{uid}'")
        bot.send_message(message.chat.id, "Вы успешно зарегистрировались!")
        start_message(message)
    except Exception as e:
        bot.send_message(message.chat.id, f"Произошла ошибка, попробуйте позже или свяжитесь с администратором {admin}")
        el.write(e)
        start_message(message)


def login_name(message):
    bot.send_message(message.chat.id, "Введите пароль")
    bot.register_next_step_handler(message, login_pass, message.text)


def login_pass(message, login):
    result = db.select("users", where=f"name = '{login}' AND pass = '{message.text}'")
    if result:
        bot.send_message(message.chat.id, "Вы успешно вошли!")
        db.update("users", ["t_id"], [f'{message.from_user.id}'], f"name = '{login}'")
        start_message(message)
    else:
        bot.send_message(message.chat.id, "Неверное имя пользователя или пароль")
        start_message(message)


def generate_unique_uid(existing_uids, length=12):
    while True:
        uid = ''.join(random.choices(string.ascii_uppercase + string.digits, k=length)).upper()
        if uid not in existing_uids:
            return uid


bot.polling(none_stop=True)
