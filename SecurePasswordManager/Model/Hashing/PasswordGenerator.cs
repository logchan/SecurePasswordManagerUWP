using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurePasswordManager.Model.Hashing
{
    public class PasswordGenerator
    {
        private IHashingServiceProvider hashingProvider;
        private List<ISaltingServiceProvider> saltingProviders;
        private List<string> saltValues;
        private IPostHashingProcessor postProcessor;

        public PasswordGenerator(IHashingServiceProvider h, IPostHashingProcessor p)
        {
            hashingProvider = h;
            postProcessor = p;
            saltingProviders = new List<ISaltingServiceProvider>();
            saltValues = new List<string>();
        }

        public void AddSalt(ISaltingServiceProvider s, string saltValue)
        {
            saltingProviders.Add(s);
            saltValues.Add(saltValue);
        }

        public string GeneratePassword(string secret, int iteration)
        {
            string result = secret;
            HashingStatus status = new HashingStatus();

            status.CurrentIteration = 0;
            for (int i = 0; i < iteration; ++i)
            {
                ++status.CurrentIteration;

                var spEnmt = saltingProviders.GetEnumerator();
                var svEnmt = saltValues.GetEnumerator();

                while(spEnmt.MoveNext() && svEnmt.MoveNext())
                {
                    result = spEnmt.Current.AddSalt(result, svEnmt.Current, status);
                } 

                result = hashingProvider.Hash(result);
            }

            return postProcessor.Process(result);
        }
    }
}
