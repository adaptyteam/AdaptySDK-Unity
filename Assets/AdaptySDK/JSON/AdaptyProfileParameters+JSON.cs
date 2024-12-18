//
//  AdaptyProfileParameters+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;
    public partial class AdaptyProfileParameters
    {
        internal JSONNode ToJSONNode()
        {
            var node = new JSONObject();

            if (FirstName != null) node.Add("first_name", FirstName);
            if (LastName != null) node.Add("last_name", LastName);
            if (Gender.HasValue) node.Add("gender", Gender.Value.ToJJSONNode());
            if (Birthday != null) node.Add("birthday", $"{Birthday.Value.Year}-{Birthday.Value.Month}-{Birthday.Value.Day}");
            if (Email != null) node.Add("email", Email);
            if (PhoneNumber != null) node.Add("phone_number", PhoneNumber);
#if UNITY_IOS
            if (AppTrackingTransparencyStatus != null) node.Add("att_status", AppTrackingTransparencyStatus.Value.ToJSON());
#endif
            if (AnalyticsDisabled != null) node.Add("analytics_disabled", AnalyticsDisabled.Value);
            if (CustomAttributes.Count > 0) node.Add("custom_attributes", CustomAttributes.ToJSONObject());

            return node;
        }
    }
}
