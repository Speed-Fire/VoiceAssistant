from g4f.client import Client

class MeaningComparator:
    
    def __init__(self):
        self.client = Client()
        
    def Compare(self, pattern, instance):
        
        txt = "Pattern: \"{pat}\".\nInstance: \"{inst}\"."

        response = self.client.chat.completions.create(
            model="gpt-3.5-turbo",
            messages=[{"role": "system", "content": "I want you to compare meanings of pattern text and instance text. If meanings of them are equal then print 'yes', otherwise 'no'. Your answer has to be only one word: yes or no."},
                      {"role": "user", "content": txt.format(pat = pattern, inst = instance)}])
        
        res = response.choices[0].message.content
        print(res)
        return res == "yes"
    
comparator = MeaningComparator()

pat = "Activate house"
instan = "Turn on the house"

res = comparator.Compare(pattern= pat, instance=instan)
print(res)
        
