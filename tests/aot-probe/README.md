# IL2CPP serialization probe

Checks on a real IL2CPP player what the Newtonsoft migration assumes from desktop behaviour.
Not part of the build; run by hand when one of those assumptions needs re-confirming.

## Running

The probe lives in a throwaway Unity project so it does not drag the SDK's native plugins into
the build:

```bash
UNITY=/Applications/Unity/Hub/Editor/6000.4.5f1/Unity.app/Contents/MacOS/Unity
PROBE=/tmp/AotProbe

"$UNITY" -batchmode -quit -createProject "$PROBE"
mkdir -p "$PROBE/Assets/Scripts" "$PROBE/Assets/Editor"
cp AotSerializationProbe.cs "$PROBE/Assets/Scripts/"
cp ProbeBuild.cs "$PROBE/Assets/Editor/"
printf '<linker>\n  <assembly fullname="Assembly-CSharp" preserve="all" />\n</linker>\n' > "$PROBE/Assets/link.xml"
# add "com.unity.nuget.newtonsoft-json": "3.2.2" to "$PROBE/Packages/manifest.json"

./run-aot-probe.sh   # adjust SP inside to point at the project
```

`run-aot-probe.sh` builds the player, swaps in the arm64 simulator runtime (Unity emits the
simulator player as x86_64 from the command line regardless of `iOSSimulatorArchitecture`),
builds the Xcode project, installs it on the booted simulator and prints the probe output.

## Results, 01.08.2026 — Unity 6000.4.5f1, IL2CPP, stripping High

```
scripting-backend -> IPhonePlayer il2cpp=True
readonly-fields   -> public-readonly=f-1 private-readonly={} readonly-int=7 readonly-enum=WinBack
required-missing  -> threw JsonSerializationException: Required property 'flow_id' not found
required-null     -> threw JsonSerializationException: Required property 'flow_id' expects a value but got null
enum-write        -> {"flow_id":"f","count":0,"offer_type":"win_back"}
dictionary-object -> n=Int64 nested=JObject
```

What this settles:

- **`readonly` fields are assigned under AOT** — public and private alike, including value types
  and enums. Keeping the SDK's 155 `readonly` fields as they are is safe.
- **`AdaptyContractResolver` works on IL2CPP**: a missing required field throws, and so does an
  explicit null once `Required.AllowNull` is raised to `Required.Always`.
- **`[EnumMember]` names are used on write.**
- **`Dictionary<string, object>` yields `Int64` and `JObject`**, same as on desktop — the SDK needs
  its own converter to keep returning `double` and nested dictionaries.
- **`link.xml` is mandatory.** The first run, without it, failed every model case with
  *"Unable to find a constructor to use for type Probe"* — stripping at High had removed the
  constructor of a type only ever created by reflection.
