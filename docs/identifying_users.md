# Identifying Users

Adapty creates an internal profile id for every user. But if you have your authentification system you should set your own Customer User Id. You can find the users by Customer User Id in [Profiles](https://docs.adapty.io/docs/profilescrm), use it in [server-side API](https://docs.adapty.io/docs/getting-started-with-server-side-api), it will be sent to all integrations.

## Setting customer user Id after configuration

You can set user id at any time with `Identify()` method. The most common cases are after registration/authorization when the user switches from being an anonymous user to an authenticated user.

```c#
Adapty.Identify("YOUR_USER_ID", (error) => {
    if(error != null) {
        // handle error
        return;
    }
    // successful identify
});
```

Request parameters:

- **User ID** (required): a string user identifier.

## Logging out and logging in

You can logout the user anytime by calling `Logout()` method:

```c#
Adapty.Logout((error) => {
    if (error != null) {
        // handle error
        return;
    }

    // successful logout
});
```

You can then login the user using `Identify()` method.