using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextService
{
    public class TextService : ITextService
    {
        void ITextService.Emit(string text)
        {
            throw new NotImplementedException();
        }
    }
}
