from g4f.client import Client

class ChatGPT:
    
    def __init__(self):
        self.systemCommand = ""
        self.model = "gpt-3.5-turbo"
        self.__client = Client()
        
    def sendMessage(self, message):
        response = self.__client.chat.completions.create(
            model=self.model,
            messages=[{"role": "system", "content": self.systemCommand},
                      {"role": "user", "content": message}])

        return response.choices[0].message.content
            




