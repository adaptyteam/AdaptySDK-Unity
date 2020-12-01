#import "Foundation/Foundation.h"

#import "AdaptyModels.mm"

static NSString *cstringToString(const char *str) {
    return str ? [NSString stringWithUTF8String:str] : nil;
}

static NSDictionary *jsonToDictionary(const char *jsonString) {
    if (jsonString) {
        NSData *jsonData = [[NSData alloc] initWithBytes:jsonString length:strlen(jsonString)];
        NSDictionary *dictionary = [NSJSONSerialization JSONObjectWithData:jsonData options:kNilOptions error:nil];
        return dictionary;
    }
    
    return nil;
}

static NSString *dictionaryToJson(NSDictionary* dictionary) {
    if (dictionary) {
        NSError *err;
        NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dictionary options:0 error:&err];
        return [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    }

    return nil;
}
