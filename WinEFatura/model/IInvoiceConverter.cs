using System;
using System.Collections.Generic;
using System.Text;

namespace com.yp.efatura.model
{
   public interface IInvoiceConverter<T, T1>
    {
       public T ToInvoice(T1 dataSource, String attachment);
       public T1 FromInvoice(T invoice);
    }
}
