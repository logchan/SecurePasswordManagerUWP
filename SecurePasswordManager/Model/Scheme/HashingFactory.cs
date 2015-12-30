using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SecurePasswordManager.Model.Hashing;

namespace SecurePasswordManager.Model.Scheme
{
    public static class HashingFactory
    {
        public static IHashingServiceProvider GetHashingServiceProvider(SPMSchemeCrypto c)
        {
            switch(c)
            {
                case SPMSchemeCrypto.MD5:
                    return new MD5Provider();
                default:
                    return new MD5Provider();
            }
        }

        public static ISaltingServiceProvider GetSaltingServiceProvider(SPMSchemeSaltingType s)
        {
            switch(s)
            {
                case SPMSchemeSaltingType.ADD_ONCE:
                    return new OnceSaltAppender();
                case SPMSchemeSaltingType.ALWAYS:
                    return new SaltAppender();
                case SPMSchemeSaltingType.ODD_ONLY:
                    return new OddOnlySaltAppender();
                case SPMSchemeSaltingType.EVEN_ONLY:
                    return new EvenOnlySaltAppender();
                default:
                    return new SaltAppender();
            }
        }

        public static IPostHashingProcessor GetPostHashingProcessor(SPMSchemeProcessType p)
        {
            switch(p)
            {
                case SPMSchemeProcessType.NO_EFFECT:
                    return new NoEffectProcessor();
                case SPMSchemeProcessType.MIXED_CASE:
                    return new MixedCasesProcessor();
                case SPMSchemeProcessType.CHARS:
                    return new HexToCharsProcessor();
                default:
                    return new NoEffectProcessor();
            }
        }

        public static PasswordGenerator GetPwdGenerator(SPMScheme scheme, List<string> salts, bool overrideProcess = false, SPMSchemeProcessType process = SPMSchemeProcessType.NO_EFFECT)
        {
            if (scheme == null || salts == null || scheme.Fields.Count != salts.Count)
                return null;

            PasswordGenerator gen = new PasswordGenerator(GetHashingServiceProvider(scheme.Crypto), 
                GetPostHashingProcessor( overrideProcess ? process : scheme.ProcessType));
            for (int i = 0; i < scheme.Fields.Count; ++i)
            {
                gen.AddSalt(GetSaltingServiceProvider(scheme.Fields[i].SaltingType), salts[i]);
            }
            
            return gen;
        }
    }
}
