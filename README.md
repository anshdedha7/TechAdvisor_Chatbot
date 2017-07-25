# TechAdvisor Chatbot

TechAdvisor Chatbot can be used by technicians at vehicle repair shops to get instant answers to questions like 'How much torque is required in compressor of BMW X5?', or 'Give me the steps to replace passenger airbags of Honda Amaze'. Edit
Add topics

## General overview

This repository provides the c# code for the chat bot which uses the bot builder sdk provided by the microsoft bot framework.
The training of NLP has been done on luis.ai platform.

The data regarding the vehicles and procedures to repair each part is stored in elasticsearch. When the bot receives a query from the user, first the requirement is parsed out by sending the query to luis with app ID and key. Then, on the basis of the requirement which has been understood, the data is fetched from the elasticsearch ad presented to the user.

```
Note: You need to add your own microsoft app Id and password in the web.config file.
```
## Dialog for chatbot
The dialog being used by the chat bot can be found on the following location :
```
/RC Assistant Bot/Dialogs/RcBotDialog.cs
```
![alt text](/images/2.png)
![alt text](/images/1.png)
