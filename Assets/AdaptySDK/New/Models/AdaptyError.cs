//
//  AdaptyError.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

namespace AdaptySDK
{
    public partial class AdaptyError
    {
        public readonly AdaptyErrorCode Code;
        public readonly string Message;
        public readonly string Detail; //nullable

        public override string ToString() => 
            $"{nameof(Code)}: {Code}, " +
            $"{nameof(Message)}: {Message}, " +
            $"{nameof(Detail)}: {Detail}";

        internal AdaptyError(AdaptyErrorCode Code, string Message, string Detail)
        {
            this.Message = Message;
            this.Detail = Detail;
            this.Code = Code;
        }
    }
}