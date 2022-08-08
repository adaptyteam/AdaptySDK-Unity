package com.adapty.unity;

import com.adapty.errors.AdaptyErrorCode;
import com.adapty.models.PeriodUnit;

class Utils {

    static int intFromErrorCode(AdaptyErrorCode code) {
        int intCode;
        switch (code) {
            case USER_CANCELED:
                intCode = 2;
                break;
            case ITEM_UNAVAILABLE:
                intCode = 5;
                break;
            case ADAPTY_NOT_INITIALIZED:
                intCode = 20;
                break;
            case PAYWALL_NOT_FOUND:
                intCode = 21;
                break;
            case PRODUCT_NOT_FOUND:
                intCode = 22;
                break;
            case INVALID_JSON:
                intCode = 23;
                break;
            case CURRENT_SUBSCRIPTION_TO_UPDATE_NOT_FOUND_IN_HISTORY:
                intCode = 24;
                break;
            case PENDING_PURCHASE:
                intCode = 25;
                break;
            case BILLING_SERVICE_TIMEOUT:
                intCode = 97;
                break;
            case FEATURE_NOT_SUPPORTED:
                intCode = 98;
                break;
            case BILLING_SERVICE_DISCONNECTED:
                intCode = 99;
                break;
            case BILLING_SERVICE_UNAVAILABLE:
                intCode = 102;
                break;
            case BILLING_UNAVAILABLE:
                intCode = 103;
                break;
            case DEVELOPER_ERROR:
                intCode = 105;
                break;
            case BILLING_ERROR:
                intCode = 106;
                break;
            case ITEM_ALREADY_OWNED:
                intCode = 107;
                break;
            case ITEM_NOT_OWNED:
                intCode = 108;
                break;
            case NO_PURCHASES_TO_RESTORE:
                intCode = 1004;
                break;
            case FALLBACK_PAYWALLS_NOT_REQUIRED:
                intCode = 1008;
                break;
            case AUTHENTICATION_ERROR:
                intCode = 2002;
                break;
            case BAD_REQUEST:
                intCode = 2003;
                break;
            case SERVER_ERROR:
                intCode = 2004;
                break;
            case REQUEST_FAILED:
                intCode = 2005;
                break;
            case MISSING_PARAMETER:
                intCode = 2007;
                break;
            default:
            case UNKNOWN:
                intCode = 0;
                break;
        }
        return intCode;
    }

    static String getPeriodString(PeriodUnit unit) {
        String periodStr;
        switch (unit) {
            case D:
                periodStr = "day";
                break;
            case W:
                periodStr = "week";
                break;
            case M:
                periodStr = "month";
                break;
            case Y:
                periodStr = "year";
                break;
            default:
                periodStr = "unknown";
                break;
        }
        return periodStr;
    }

    private Utils() {}
}
