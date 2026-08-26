#!/usr/bin/env python3
"""Mechanical half of the contract-conformance check.

Reads cross_platform.yaml and the C# sources and prints structural differences. It decides
nothing: every line it prints is a candidate to confirm by reading the source, and everything it
cannot map is handed back for a manual pass.

Usage:  python3 extract.py <repo-root> [--json out.json]
"""
import json
import pathlib
import re
import sys

try:
    import yaml
except ImportError:
    sys.exit("pyyaml missing. Run:  python3 -m venv /tmp/cc-venv && /tmp/cc-venv/bin/pip install pyyaml")

ROOT = pathlib.Path(sys.argv[1] if len(sys.argv) > 1 and not sys.argv[1].startswith("-") else ".")
SRC = ROOT / "Packages/com.adapty.unity-sdk/Runtime"

# Contract objects whose C# name is not derivable from the contract name. Extend as the contract
# grows - anything absent from here and not auto-matched is reported as unmapped, loudly.
MAPPING = {
    "CustomerIdentityParameters": "AdaptyCustomerIdentity",
    "AdaptyPaywallProduct.Response": "AdaptyPaywallProduct",
    "AdaptyPaywallProduct.Request": "AdaptyPaywallProductRequest",
    # Renamed from AdaptyPaywallProduct.Subscription in contract 4.1.0, now shared with the
    # promoted product.
    "AdaptyProduct.Subscription": "AdaptySubscription",
    "AdaptyPromotedProduct.Response": "AdaptyPromotedProduct",
    "AdaptyPromotedProduct.Request": "AdaptyPromotedProductRequest",
    "AdaptySubscriptionOffer.Phase": "AdaptySubscriptionPhase",
    "AdaptyUI.FlowView": "AdaptyUIFlowView",
    "AdaptyUI.OnboardingView": "AdaptyUIOnboardingView",
    "AdaptyUI.OnboardingMeta": "AdaptyUIOnboardingMeta",
    "AdaptyUI.DialogConfiguration": "AdaptyUIDialogConfiguration",
    "AdaptyUI.UserAction": "AdaptyUIUserAction",
    "AdaptyProfile.AccessLevel": "AdaptyProfile+AccessLevel",
    "AdaptyProfile.NonSubscription": "AdaptyProfile+NonSubscription",
    "AdaptyProfile.Subscription": "AdaptyProfile+Subscription",
    "AdaptyFlowPaywall.ProductReference": "AdaptyFlowPaywall+ProductReference",
    # $assets variants: one contract object, several C# types. Their members are unioned.
    "Color": "AdaptyCustomAssetColor",
    "ColorGradient": "AdaptyCustomAssetLinearGradient",
    "Image": ["AdaptyCustomAssetLocalImageAsset", "AdaptyCustomAssetLocalImageFile",
              "AdaptyCustomAssetLocalImageData"],
    "Video": ["AdaptyCustomAssetLocalVideoAsset", "AdaptyCustomAssetLocalVideoFile"],
}

# Contract objects that deliberately have no [DataMember] type behind them. The reason is the
# point: it is what a reviewer checks. Anything here is still worth a look by hand.
NO_MODEL = {
    "AdaptySubscriptionOffer": "built by AdaptyConverterSubscriptionOffer - check the converter",
    "AdaptySubscriptionOffer.Identifier": "flattened into AdaptySubscriptionOffer by the converter",
    "AdaptyInstallationStatus": "polymorphic, built by AdaptyConverterInstallationStatus",
    "AdaptyUI.OnboardingsStateParams": "polymorphic, built by AdaptyConverterOnboardingsStateUpdatedParams",
    "AdaptyProfile.CustomAttributes": "free-form map, not a type",
    "AdaptyUI.CustomTagsValues": "free-form map",
    "AdaptyUI.CustomTimersValues": "free-form map",
    "AdaptyUI.ProductPurchaseParameters": "free-form map",
    "AdaptyUI.CustomAssets": "array of $assets variants",
}

ENUM_MAPPING = {
    "AdaptyLog.Level": "AdaptyLogLevel",
    "AdaptyProfile.Gender": "AdaptyProfileGender",
    "AdaptySubscriptionPeriod.Unit": "AdaptySubscriptionPeriodUnit",
    "AdaptySubscriptionOffer.PaymentMode": "AdaptyPaymentMode",
    "AdaptySubscriptionOffer.Identifier.Type": "AdaptySubscriptionOfferType",
    "AdaptyWebPresentation": "AdaptyWebPresentation",
    "AdaptyUI.DialogActionType": "AdaptyUIDialogActionType",
}


# --------------------------------------------------------------------------- contract
def flatten(node):
    """Merge a schema object with its oneOf branches.

    oneOf hides properties inside the branches: reading only the top level makes every C# member
    look invented. required_all is what every branch requires, required_any what any branch does.
    """
    props, plat = {}, {}
    top_req = set(node.get("required") or [])
    branch_reqs = []

    def take(d):
        for k, v in (d.get("properties") or {}).items():
            props[k] = v
            desc = v.get("description", "") if isinstance(v, dict) else ""
            plat[k] = "ios" if "iOS Only" in desc else "android" if "Android Only" in desc else None

    take(node)
    for b in node.get("oneOf") or []:
        if isinstance(b, dict):
            take(b)
            branch_reqs.append(set(b.get("required") or []))

    if branch_reqs:
        return props, top_req | set.intersection(*branch_reqs), top_req | set.union(*branch_reqs), plat
    return props, top_req, top_req, plat


def const_of(node, key):
    for src in [node] + (node.get("oneOf") or []):
        if isinstance(src, dict):
            p = (src.get("properties") or {}).get(key)
            if isinstance(p, dict) and "const" in p:
                return p["const"]
    return None


def load_contract(path):
    doc = yaml.safe_load(path.read_text())
    types, enums, envelopes = {}, {}, {}
    for section in ("$defs", "$assets"):
        for name, node in (doc.get(section) or {}).items():
            if not isinstance(node, dict):
                continue
            if node.get("type") == "string" and node.get("enum"):
                enums[name] = set(node["enum"])
                continue
            props, req_all, req_any, plat = flatten(node)
            if props:
                types[name] = {"section": section, "props": props, "required_all": req_all,
                               "required_any": req_any, "platform": plat}
    for section in ("$requests", "$events"):
        for name, node in (doc.get(section) or {}).items():
            if not isinstance(node, dict):
                continue
            props, req_all, _, _ = flatten(node)
            wire = const_of(node, "method") or const_of(node, "id")
            envelopes[f"{section}/{name}"] = {"wire": wire, "props": sorted(props),
                                              "required": sorted(req_all)}
    return doc, types, enums, envelopes


# --------------------------------------------------------------------------- C#
CLASS = re.compile(r'^(\s*)(?:public|internal|private|protected)\s+(?:static\s+|sealed\s+|abstract\s+|partial\s+)*class\s+(\w+)')
DATAMEMBER = re.compile(r'\[DataMember\(([^\]]*)\)\]')
NAME = re.compile(r'Name\s*=\s*"([^"]+)"')
ISREQ = re.compile(r'IsRequired\s*=\s*true')
ENUM = re.compile(r'public enum (\w+)\s*(?::\s*\w+\s*)?\{(.*?)\n(\s*)\}', re.S)
ENUMMEMBER = re.compile(r'\[EnumMember\(Value\s*=\s*"([^"]+)"\)\]')


def load_csharp(src):
    members, enums, cases, strings = {}, {}, {}, {}
    for f in sorted(src.rglob("*.cs")):
        text = f.read_text()
        rel = str(f.relative_to(src.parent.parent.parent))

        for m in ENUM.finditer(text):
            vals = set(ENUMMEMBER.findall(m.group(2)))
            enums[m.group(1)] = {"values": vals, "file": rel, "numeric": not vals}

        cls_stack, indent_stack, guard, pending = [], [], None, None
        for lineno, line in enumerate(text.splitlines(), 1):
            s = line.strip()
            if s.startswith("#if"):
                guard = "ios" if "UNITY_IOS" in s else "android" if "UNITY_ANDROID" in s else guard
                continue
            if s.startswith(("#endif", "#else")):
                guard = None
                continue

            c = CLASS.match(line)
            if c:
                ind = len(c.group(1))
                while indent_stack and indent_stack[-1] >= ind:
                    cls_stack.pop(); indent_stack.pop()
                cls_stack.append(c.group(2)); indent_stack.append(ind)

            d = DATAMEMBER.search(line)
            if d:
                n = NAME.search(d.group(1))
                if n:
                    pending = (n.group(1), bool(ISREQ.search(d.group(1))), guard, lineno)
                continue
            if pending and s and not s.startswith(("//", "[", "///", "#")):
                key, required, g, ln = pending
                members.setdefault("+".join(cls_stack) or f.stem, {})[key] = {
                    "required": required, "platform": g, "file": rel, "line": ln}
                pending = None

            for v in re.findall(r'case\s+"([^"]+)"', line):
                cases.setdefault(f.stem, set()).add(v)
            for v in re.findall(r'"([a-z][a-z0-9_.]{1,})"', line):
                strings.setdefault(v, set()).add(f"{rel}:{lineno}")
    return members, enums, cases, strings


def main():
    doc, types, cenums, envelopes = load_contract(ROOT / "cross_platform.yaml")
    members, csenums, cases, strings = load_csharp(SRC)

    keys = sum(len(t["props"]) for t in types.values())
    print(f"contract: {len(types)} object types ({keys} keys), {len(cenums)} string enums, "
          f"{len(envelopes)} request/event envelopes")
    print(f"C#:       {len(members)} types with [DataMember], {len(csenums)} enums")
    if len(types) < 25 or keys < 150 or len(envelopes) < 80:
        sys.exit("\nSTOP: the walk found implausibly little. Fix extraction before trusting anything below.")

    pairs, unmapped = {}, []
    for name in types:
        t = MAPPING.get(name) or (name if name in members else None)
        ts = [t] if isinstance(t, str) else (t or [])
        ts = [x for x in ts if x in members]
        if ts:
            pairs[name] = ts
        elif name not in NO_MODEL:
            unmapped.append(name)

    print(f"\nmapped {len(pairs)}, no model by design {len(NO_MODEL)}, unmapped {len(unmapped)}")

    if unmapped:
        print("\n" + "=" * 72)
        print("UNMAPPED - map each one in MAPPING, or add it to NO_MODEL with a reason")
        print("=" * 72)
        for c in unmapped:
            print(f"  {c}   keys: {sorted(types[c]['props'])[:6]}")

    orphans = sorted(set(members) - {t for ts in pairs.values() for t in ts})
    if orphans:
        print("\n" + "=" * 72)
        print("C# TYPES WITH [DataMember] AND NO CONTRACT OBJECT")
        print("=" * 72)
        for t in orphans:
            print(f"  {t}")

    print("\n" + "=" * 72)
    print("STRUCTURAL DIFFERENCES - candidates, confirm each by reading the source")
    print("=" * 72)
    n = 0
    for cname, tnames in sorted(pairs.items()):
        cd = types[cname]
        md = {}
        for tn in tnames:
            for k, v in members[tn].items():
                md.setdefault(k, v)
        tname = " | ".join(tnames)
        miss = sorted(set(cd["props"]) - set(md))
        extra = sorted(set(md) - set(cd["props"]))
        both = sorted(set(cd["props"]) & set(md))
        union = len(tnames) > 1
        reqd = [] if union else [k for k in both if (k in cd["required_all"]) != md[k]["required"]]
        platd = [k for k in both if cd["platform"].get(k) != md[k]["platform"]]
        if not (miss or extra or reqd or platd):
            continue
        n += 1
        print(f"\n{cname}  ->  {tname}")
        for k in miss:
            hint = f"  [{cd['platform'][k]} only]" if cd["platform"].get(k) else ""
            # Converters are the usual explanation for a key with no attribute, so show them first.
            hits = sorted(strings.get(k, []), key=lambda s: ("Serialization/" not in s, s))
            print(f"   contract key with no [DataMember]: {k}{hint}"
                  + (f"   (string appears at {hits[:2]}, {len(hits)} site(s) total)"
                     if hits else "   (string appears nowhere)"))
        for k in extra:
            print(f"   [DataMember] with no contract key: {k}   ({md[k]['file']}:{md[k]['line']})")
        for k in reqd:
            c_req = k in cd["required_all"]
            print(f"   required: {k} - contract={'required' if c_req else 'optional'}, "
                  f"C#={'IsRequired' if md[k]['required'] else 'optional'}"
                  f"   ({md[k]['file']}:{md[k]['line']})"
                  + ("   [conditional: required only in some oneOf branches]"
                     if not c_req and k in cd["required_any"] else ""))
        for k in platd:
            print(f"   platform: {k} - contract={cd['platform'].get(k) or 'none'}, "
                  f"C# #if={md[k]['platform'] or 'none'}   ({md[k]['file']}:{md[k]['line']})")
    if not n:
        print("\n  none")

    print("\n" + "=" * 72)
    print("STRING ENUMS")
    print("=" * 72)
    for name, vals in sorted(cenums.items()):
        cs_name = ENUM_MAPPING.get(name, name.replace(".", ""))
        cs = csenums.get(cs_name)
        if cs is None:
            print(f"\n  {name}: {sorted(vals)}\n    NO C# ENUM MATCHED - map it in ENUM_MAPPING")
            continue
        extra, miss = sorted(cs["values"] - vals), sorted(vals - cs["values"])
        flag = "  <-- DIFFERS" if (extra or miss) else ""
        print(f"\n  {name} -> {cs_name}{flag}")
        print(f"    contract: {sorted(vals)}")
        print(f"    C#:       {sorted(cs['values'])}")
        if extra:
            print(f"    EXTRA IN C#: {extra}   (fallback member? then check the WRITE path)")
        if miss:
            print(f"    MISSING IN C#: {miss}")

    print("\n" + "=" * 72)
    print("WIRE NAMES - every method/event id in the contract, and whether it appears in C#")
    print("=" * 72)
    absent = [(n, e["wire"]) for n, e in sorted(envelopes.items())
              if e["wire"] and e["wire"] not in strings]
    print(f"  {len(envelopes)} envelopes, {sum(1 for e in envelopes.values() if e['wire'])} carry a const name")
    if absent:
        for n, w in absent:
            print(f"   NOT FOUND IN C#: {w}   ({n})")
    else:
        print("   every const method/event name appears somewhere in Runtime/")

    print("\n" + "=" * 72)
    print("CONVERTER case LABELS - compare against the oneOf discriminators by hand")
    print("=" * 72)
    for f, vals in sorted(cases.items()):
        print(f"  {f}: {sorted(vals)}")

    if "--json" in sys.argv:
        out = sys.argv[sys.argv.index("--json") + 1]
        json.dump({"types": {k: {"props": sorted(v["props"]), "required_all": sorted(v["required_all"]),
                                 "required_any": sorted(v["required_any"]), "platform": v["platform"]}
                             for k, v in types.items()},
                   "members": members, "envelopes": envelopes,
                   "contract_enums": {k: sorted(v) for k, v in cenums.items()},
                   "csharp_enums": {k: {"values": sorted(v["values"]), "numeric": v["numeric"]}
                                    for k, v in csenums.items()},
                   "pairs": pairs, "unmapped": unmapped}, open(out, "w"), indent=1)
        print(f"\nwrote {out}")


if __name__ == "__main__":
    main()
