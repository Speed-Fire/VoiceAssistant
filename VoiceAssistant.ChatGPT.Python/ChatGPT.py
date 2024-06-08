from g4f.client import Client
from g4f.Provider import You

class ChatGPT:
    
    def __init__(self):
        self.systemCommand = ""
        self.model = "gpt-3.5-turbo"
        self.__client = Client(provider=You)
        
    def sendMessage(self, message):
        response = self.__client.chat.completions.create(
            model=self.model,
            messages=[{"role": "system", "content": self.systemCommand},
                      {"role": "user", "content": message}])

        return response.choices[0].message.content
    
chat = ChatGPT()

res = chat.sendMessage('What it will be two times two?')
            




