# About
This demonstrates a bug (or perhaps simply odd behavior) in [HotChocolate](https://chillicream.com/docs/hotchocolate/v14) 14.1.0 when adding codes to errors using error filters.

Given that the `IError` type has a `Code` property, and it is not uncommon for errors to have both a message and a code, I would expect calling the `.WithCode("CUSTOM_ERROR_CODE")` extension method would, at a minimum, cause a `code` field to be present in the returned JSON response.

Instead, the error code gets added **only** to the extensions collection, but not as a stand-alone error code.

#### Expectation
```json
{
  "errors": [
    {
      "message": "Unexpected Execution Error",
      "code": "CUSTOM_ERROR_CODE",
      "locations": [
        {
          "line": 2,
          "column": 3
        }
      ],
      "path": [
        "testError"
      ]
    }
  ],
  "data": null
}
```

#### Reality
```json
{
  "errors": [
    {
      "message": "Unexpected Execution Error",
      "locations": [
        {
          "line": 2,
          "column": 3
        }
      ],
      "path": [
        "testError"
      ],
      "extensions": {
        "code": "CUSTOM_ERROR_CODE"
      }
    }
  ],
  "data": null
}
```

There are several downsides to this approach:
1. It requires more effort on the part of clients to drill down into the extensions to find error codes, and coding logic against extensions seems messier than expecting a code alongside the message for each configured error.
2. It disallows the error code to be distinct from the extension code. For example, if an error with a code is returned that wraps another error that also has a code of its own.
3. It is functionally misleading, as calling the `.RemoveExtensions()` method on the `IError` does not, in fact, clear all extensions when an error code has been set.

## Steps to Reproduce
1. Set up an ASP.Net web app with `HotChocolate.AspNetCore` installed.
2. Configure HotChocolate to use an error filter on the query.
3. Set an error code for filtered errors within the error filter.
4. Throw an exception from within a query.
5. Observe that the error code is only present in the extensions and **not** as a stand-alone field on the top-level error response.

## Desired Change
Please provide a way to set an error code for filtered errors that is present in the top-level `errors` collection, **independent** of extensions.

## Using the Demo
1. Navigate to `HotChocolateBugDemo`
2. Run the `dotnet run` command
3. Open a browser to `{Now listening on URL here}/graphql`
4. Make a call to the `testError` query and observe the results

```graphql
query {
  testError
}
```