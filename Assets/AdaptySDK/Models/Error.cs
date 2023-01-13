//
//  Error.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

namespace AdaptySDK
{
    public static partial class Adapty
    {
        public partial class Error
        {
            public readonly ErrorCode Code;
            public readonly string Message;
            public readonly string Detail;

            public override string ToString()
            {
                return $"{nameof(Code)}: {Code}, " +
                       $"{nameof(Message)}: {Message}, " +
                       $"{nameof(Detail)}: {Detail}";
            }

            internal Error(ErrorCode Code, string Message, string Detail)
            {
                this.Message = Message;
                this.Detail = Detail;
                this.Code = Code;
            }
        }
    }
}

