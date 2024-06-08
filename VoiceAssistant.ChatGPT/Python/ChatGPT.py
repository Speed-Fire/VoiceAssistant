import os.path
from g4f.client import Client
from g4f.Provider import You, Liaobots, RetryProvider, Gemini
from g4f.cookies import set_cookies_dir, read_cookie_files

class ChatGPT:
    
    def __init__(self):
        self.systemCommand = ""
        self.model = ""
        self.__client = Client(provider=RetryProvider(
            providers=[Gemini, Liaobots], 
            single_provider_retry=True)
                               ) #Client(provider=You)
        
    def sendMessage(self, message):
        response = self.__client.chat.completions.create(
            model="gpt-3.5-turbo",
            messages=[{"role": "system", "content": self.systemCommand},
                      {"role": "user", "content": message}])
        
        return response.choices[0].message.content
    

def CreateChatGpt():
    return ChatGPT()

def SetupCookies():
    path = os.path.join(os.path.dirname(__file__), "har_and_cookies")
    
            




