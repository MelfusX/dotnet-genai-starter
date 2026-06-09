# Cost Tracking

GenAI features are cost-sensitive. The starter kit tracks usage and estimates cost from day one.

## Inputs

- input tokens;
- output tokens;
- embedding tokens;
- model pricing;
- request count.

## Usage Endpoint

```http
GET /api/v1/usage?from=2026-04-01&to=2026-04-30
```

Supported filters:

- `from`
- `to`
- `userId`
- `tenantId`
- `model`

## Response Shape

```json
{
  "requests": 4300,
  "inputTokens": 1200000,
  "outputTokens": 300000,
  "embeddingTokens": 900000,
  "estimatedCost": 87.42,
  "currency": "USD"
}
```

## Pricing

The observability schema stores pricing records in `genai.ai_model_pricing`. Records include:

- provider;
- model;
- currency;
- input token price;
- output token price;
- embedding token price where applicable;
- effective dates.

When a model request is logged, estimated cost is calculated from the pricing
record effective at the request timestamp and stored on the request log. Usage
queries sum those stored estimates, so historical usage remains reproducible
after pricing changes.

The local mock models are seeded with zero-cost USD pricing for deterministic
local runs and tests.

## Quotas

Future quota examples:

- max requests per user per day;
- max tokens per user per day;
- max estimated cost per user per month;
- max requests per tenant per day, optional.
