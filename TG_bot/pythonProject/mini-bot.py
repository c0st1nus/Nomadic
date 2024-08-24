import telebot
from telebot import types
bot = telebot.TeleBot('7146246279:AAF0pR-XCaLpYhtG3LKVs2CHjA_XQI7MG-4')
admins = [
    '1239398217',
    '6408624738'
]
@bot.message_handler(commands=['start'])
def start_message(message):
    keyboard = types.ReplyKeyboardMarkup(resize_keyboard=True)
    button = types.KeyboardButton("Получить apk")
    button2 = types.KeyboardButton("Оставить обратную связь")
    keyboard.add(button, button2)
    bot.send_message(message.chat.id, "Здравствуйте! Я бот проекта Nomadic. Вы можете получить доступ к бета-версии игры нажав на кнопку \"Получить apk\"", reply_markup=keyboard)

@bot.message_handler(content_types=['text'])
def get_text_messages(message):
    if message.text == "Получить apk":
        with open('path/to/apk/file.apk', 'rb') as apk_file:
            bot.send_document(message.from_user.id, apk_file, caption="Вот ваш apk файл")
        bot.send_message(message.from_user.id, "Поддержать наш проект вы можете через [kaspi](https://pay.kaspi.kz/pay/u3moldyc)", parse_mode='Markdown')
    elif message.text == "Обратная связь":
        bot.register_next_step_handler(message, feedback)

def feedback(message):
    if message.text != " ":
        bot.send_message(message.chat.id, "Спасибо большое за обратную связь❤️!!!")
        for admin in admins:
            bot.send_message(admin, f"Пользователь {message.from_user.id} оставил обратную связь: {message.text}")
    else:
        bot.send_message(message.chat.id, "Вы не оставили обратную связь. Попробуйте еще раз")

bot.polling(none_stop=True)