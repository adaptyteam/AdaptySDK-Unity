---
name: contract-conformance
description: Use when checking that the Unity SDK's C# models, enums and converters still match cross_platform.yaml — before a release, after bumping the native SDKs or the contract version, or when a payload behaves differently than the contract says it should
---

# Contract conformance

`cross_platform.yaml` is the canonical description of every request, response and event that
crosses the bridge. The C# side restates it a second time, in attributes and hand-written
converters. Nothing in the build compares the two, so they drift apart silently.

This skill is that comparison. `extract.py`, next to this file, does the mechanical half; the rest
is reading and judgement, and that is where the findings actually are.

Work through the steps in order. Do not skip step 5 — the mechanical half agrees with itself and
still misses most of what matters.

## What this cannot tell you

Put these in the report rather than leaving them implied:

- **The contract may be wrong, or ahead of the implementations.** It is maintained in AdaptySDK-iOS
  and describes all platforms. A key can be declared there and implemented nowhere.
- **Error codes are not in the contract at all.** `AdaptyErrorCode` can only be checked against the
  native Swift and Kotlin sources. Different job, do not attempt it here.
- **Value semantics** — units, ranges, what a value means. Shape and naming only.

  A `format` declared in the contract is the exception: it is part of the shape. If a key says
  `format: "YYYY-MM-dd"`, whether the C# side actually produces that is in scope, and reading the
  code may not settle it — hand-built strings are where this goes wrong. Serialize a value and look
  at the bytes. Fixtures will not save you: a snapshot whose only date is `1815-12-10` passes
  whether or not the writer pads single digits.

## Step 1. Run the mechanical pass

```bash
python3 -m venv /tmp/cc-venv && /tmp/cc-venv/bin/pip install --quiet pyyaml
```

```bash
/tmp/cc-venv/bin/python .claude/skills/contract-conformance/extract.py . --json /tmp/cc.json
```

It refuses to run if its own walk found implausibly little, so a clean exit means the input was
read. Read its first two lines and sanity-check the counts before trusting anything after them.

## Step 2. Check the copy of the contract

The repo's copy has to match the canonical one in AdaptySDK-iOS. If `.ios-sdk/` is checked out
(see the `ios-sdk-reference` skill) this is one command, not an act of faith:

```bash
diff .ios-sdk/Sources.AdaptyPlugin/cross_platform.yaml cross_platform.yaml && echo IDENTICAL
```

If `.ios-sdk/` is missing, say in the report that this was not verified. Do not claim it matches.

## Step 3. Triage what the script printed

Every line it prints is a **candidate**, not a finding. Confirm each one by opening the file at the
line it names. Expect a good share to dissolve on contact — that is the script working as intended,
not failing.

- **UNMAPPED** — a contract object with no C# type matched. Either map it in `MAPPING` or add it to
  `NO_MODEL` with a reason, then rerun. Never leave one unexplained: a new contract object landing
  unmapped is exactly the signal this whole exercise exists for.
- **C# TYPES WITH NO CONTRACT OBJECT** — usually request-shaping helpers and nested holders. For
  each, satisfy yourself it is internal plumbing and not an invented wire shape.
- **contract key with no `[DataMember]`** — the script also tells you whether that string appears
  anywhere in `Runtime/`. If it does, the key is probably supplied by a converter rather than an
  attribute, and there is no defect; go read that line. If it appears nowhere, you likely have one.
- **required mismatch** — a key the contract requires in every `oneOf` branch that C# does not mark
  `IsRequired`, or the reverse. Keys required in only some branches are annotated as conditional and
  are usually fine.
- **platform** — the contract marks a key iOS/Android Only and C# does not gate it behind `#if`, or
  the reverse. **Before calling this a defect, grep `CHANGELOG.md`, the surrounding comments and the
  tests.** Some of these are deliberate: request-side parameter objects are intentionally ungated so
  one call site compiles for every target.
- **STRING ENUMS** — a member in C# that the contract does not list is a defect. There is no
  fallback: an unlisted string fails the read, and the only `Unknown` members left are the two the
  contract spells out itself, on `AdaptyPaymentMode` and `AdaptySubscriptionPeriodUnit`. The reverse
  is a defect too — a contract value with no member — and so is a member with no
  `[EnumMember]`, which would be sent under its C# name.
- **WIRE NAMES** — every `method`/`id` constant in the contract, and whether that literal occurs in
  `Runtime/`. An absent one is a request the SDK cannot make or an event it cannot receive.

## Step 4. Cover what the script only lists

The script prints converter `case` labels but does not compare them. Do it by hand: for every
`oneOf` in the contract that is not the generic `error`/`success` envelope, find its discriminator
values and match them against the converter that reads that type, or against the string literals in
the model when the discriminator is chosen by a static factory rather than a `switch`.

Also read the converters in `Runtime/Serialization/` directly, one by one. Types built by a
converter have no `[DataMember]` at all, so nothing in step 3 says anything about them, and each
converter is a hand-written restatement of a contract object. For each key the converter reads, ask
whether the contract requires it and whether the converter enforces that — `JsonRequire.*` enforces,
a plain `node["x"]?` does not.

## Step 5. Ask the questions the script cannot

This is where the findings are.

- **The write path is not the read path.** For every value C# can produce, ask: can it be serialized
  into a request, and does the contract allow it *there*? Requests often have their own contract
  object, distinct from the response object of the same name — an enum listed in a response may have
  fewer values allowed in the request that carries it back.
- **Optional keys the C# side cannot express.** A contract key that is optional and absent from C#
  breaks nothing, and is still a capability the SDK does not offer.
- **Enforcement, not just presence.** A required key that is read leniently is a conformance gap even
  though the property exists.

## Rules that are not optional

Two failure modes have already produced confident, wrong reports. Guard against both explicitly.

1. **Re-read every line you cite.** Before using `file:line` as evidence — in your reasoning or in
   the report — open that exact line and confirm it says what you think. A single misread line
   reference has produced an entire well-argued finding about a divergence that did not exist, with
   correct supporting quotes from native sources hung off a false premise.
2. **A rule inferred from a sample must be checked against the whole family.** If you conclude
   "these enums all behave this way" or "this convention holds everywhere", enumerate every member
   and check each. The exception is what you were looking for. A run that established a project-wide
   rule from five of six enums missed the sixth, which was the only real finding in that area.
3. **When any line in a method draws your attention, read the whole method.** Whatever led you there
   — a script candidate, a failing test, a diff — is not the only thing in it. A run that correctly
   reported a lenient read of a required key missed a second lenient read of another required key
   fifteen lines below it, in the same method, because only the first one had drawn attention.

And: prefer a hedged finding to a confident one. If you cannot tell whether something is deliberate,
say so and say what you looked at — that is a useful report. A wrong classification sends someone
into three repositories after nothing.

## Step 6. Report

One entry per finding, each carrying:

- **evidence on both sides** — `cross_platform.yaml` line, and `file.cs:line`, each re-read per
  rule 1;
- **classification**:
  - *our defect* — contract and native SDKs agree, the Unity side does not;
  - *question for the contract owners* — the contract says something no implementation does, or the
    implementations disagree with it. Check the native sources before choosing this over the first;
  - *deliberate divergence* — differs on purpose, e.g. a Unity type with no wire equivalent, or
    behaviour preserved from v3. Say what pins it: a comment, a test, a changelog entry.
- **whether it can fire today**, or only once a native SDK adds something.

Close with coverage: how many contract objects were compared, how many keys, what was skipped and
why. A report that does not say what it did not look at is not finished.

## Not your call

Do not edit `cross_platform.yaml` — it is a copy of the canonical file, and changing it here only
hides the divergence.

Do not add a member to a model because the contract has one, or remove one because it does not.
Either can be a public API change, and either may be the contract's mistake rather than ours.
Report, classify, stop.
