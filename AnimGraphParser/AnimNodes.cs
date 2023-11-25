using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI.UnrealTypes;

namespace AnimGraphParser
{
    public class AnimNode_Base
    {
        public string NodeName { get; private set; }
        public string NodeStructName { get; private set; }
        public string NodeGraphName { get; set; }
        public List<KeyValuePair<string, AnimNode_Base>> InputNodes { get; private set; }

        public AnimNode_Base(FName NodeName, FName NodeStructName) 
        {
            this.NodeName = NodeName.ToString();
            this.NodeStructName = NodeStructName.ToString();
            NodeGraphName = "UNKNOWN_ANIM_NODE";

            InputNodes = new List<KeyValuePair<string, AnimNode_Base>>();
        }

        public AnimNode_Base(FName NodeName)
        {
            /** Unknown node struct */

            this.NodeName = NodeName.ToString();
            NodeStructName = "UNKNOWN_ANIM_NODE_STRUCT";
            NodeGraphName = "UNKNOWN_ANIM_NODE";

            InputNodes = new List<KeyValuePair<string, AnimNode_Base>>();
        }

        protected void AddInputNode(string PropertyName, AnimNode_Base AnimNode)
        {
            InputNodes.Add(new KeyValuePair<string, AnimNode_Base>(PropertyName, AnimNode));
        }

        protected StructPropertyData GetNodeStructFromPoseLink(StructPropertyData PoseLink)
        {
            IntPropertyData? LinkID = Program.FindPropertyInStruct(PoseLink, "LinkID") as IntPropertyData;
            if (LinkID is null) throw new Exception("Failed to get LinkID");

            return Program.AnimNodeProperties[LinkID.Value];
        }

        public static AnimNode_Base ConvertStructToAnimNode(StructPropertyData AnimNodeStruct)
        {
            string StructType = AnimNodeStruct.StructType.ToString();

            return StructType switch
            {
                "AnimNode_ApplyAdditive" => new AnimNode_ApplyAdditive(AnimNodeStruct),
                "AnimNode_ApplyMeshSpaceAdditive" => new AnimNode_ApplyMeshSpaceAdditive(AnimNodeStruct),
                "AnimNode_BlendSpacePlayer" => new AnimNode_BlendSpacePlayer(AnimNodeStruct),
                "AnimNode_AimOffsetLookAt" => new AnimNode_AimOffsetLookAt(AnimNodeStruct),
                "AnimNode_RotationOffsetBlendSpace" => new AnimNode_RotationOffsetBlendSpace(AnimNodeStruct),
                "AnimNode_SequenceEvaluator" => new AnimNode_SequenceEvaluator(AnimNodeStruct),
                "AnimNode_SequencePlayer" => new AnimNode_SequencePlayer(AnimNodeStruct),
                "AnimNode_BlendListByBool" => new AnimNode_BlendListByBool(AnimNodeStruct),
                "AnimNode_BlendListByEnum" => new AnimNode_BlendListByEnum(AnimNodeStruct),
                "AnimNode_BlendListByInt" => new AnimNode_BlendListByInt(AnimNodeStruct),
                "AnimNode_ConvertComponentToLocalSpace" => new AnimNode_ConvertComponentToLocalSpace(AnimNodeStruct),
                "AnimNode_ConvertLocalToComponentSpace" => new AnimNode_ConvertLocalToComponentSpace(AnimNodeStruct),
                "AnimNode_LinkedAnimLayer" => new AnimNode_LinkedAnimLayer(AnimNodeStruct),
                "AnimNode_LinkedAnimGraph" => new AnimNode_LinkedAnimGraph(AnimNodeStruct),
                "AnimNode_LayeredBoneBlend" => new AnimNode_LayeredBoneBlend(AnimNodeStruct),
                "AnimNode_LinkedInputPose" => new AnimNode_LinkedInputPose(AnimNodeStruct),
                "AnimNode_MeshSpaceRefPose" => new AnimNode_MeshSpaceRefPose(AnimNodeStruct),
                "AnimNode_MultiWayBlend" => new AnimNode_MultiWayBlend(AnimNodeStruct),
                "AnimNode_RandomPlayer" => new AnimNode_RandomPlayer(AnimNodeStruct),
                "AnimNode_RefPose" => new AnimNode_RefPose(AnimNodeStruct),
                "AnimNode_Root" => new AnimNode_Root(AnimNodeStruct),
                "AnimNode_StateResult" => new AnimNode_StateResult(AnimNodeStruct),
                "AnimNode_SaveCachedPose" => new AnimNode_SaveCachedPose(AnimNodeStruct),
                "AnimNode_AnimDynamics" => new AnimNode_AnimDynamics(AnimNodeStruct),
                "AnimNode_Fabrik" => new AnimNode_Fabrik(AnimNodeStruct),
                "AnimNode_ModifyBone" => new AnimNode_ModifyBone(AnimNodeStruct),
                "AnimNode_SpringBone" => new AnimNode_SpringBone(AnimNodeStruct),
                "AnimNode_TwoBoneIK" => new AnimNode_TwoBoneIK(AnimNodeStruct),
                "AnimNode_Slot" => new AnimNode_Slot(AnimNodeStruct),
                "AnimNode_StateMachine" => new AnimNode_StateMachine(AnimNodeStruct),
                "AnimNode_TransitionResult" => new AnimNode_TransitionResult(AnimNodeStruct),
                "AnimNode_TwoWayBlend" => new AnimNode_TwoWayBlend(AnimNodeStruct),
                "AnimNode_UseCachedPose" => new AnimNode_UseCachedPose(AnimNodeStruct),

                _ => new AnimNode_Base(AnimNodeStruct.Name)
            };
        }

        public virtual string GetNodeInfo()
        {
            return $"{NodeGraphName} ({NodeName})";
        }
    }

    public class AnimNode_ApplyAdditive : AnimNode_Base
    {
        public AnimNode_ApplyAdditive(StructPropertyData AnimNodeStruct)
            : base(AnimNodeStruct.Name, AnimNodeStruct.StructType) 
        {
            NodeGraphName = "Apply Additive";

            StructPropertyData? Base = Program.FindPropertyInStruct(AnimNodeStruct, "Base") as StructPropertyData;
            StructPropertyData? Additive = Program.FindPropertyInStruct(AnimNodeStruct, "Additive") as StructPropertyData;
            if (Base is null || Additive is null) throw new Exception("Failed to get Base/Additive PoseLinks");

            StructPropertyData BaseNodeStruct = GetNodeStructFromPoseLink(Base);
            StructPropertyData AdditiveNodeStruct = GetNodeStructFromPoseLink(Additive);

            AddInputNode("Base", ConvertStructToAnimNode(BaseNodeStruct));
            AddInputNode("Additive", ConvertStructToAnimNode(AdditiveNodeStruct));
        }
    }

    public class AnimNode_ApplyMeshSpaceAdditive : AnimNode_Base
    {
        public AnimNode_ApplyMeshSpaceAdditive(StructPropertyData AnimNodeStruct)
            : base(AnimNodeStruct.Name, AnimNodeStruct.StructType)
        {
            NodeGraphName = "Apply Additive Mesh Space";

            StructPropertyData? Base = Program.FindPropertyInStruct(AnimNodeStruct, "Base") as StructPropertyData;
            StructPropertyData? Additive = Program.FindPropertyInStruct(AnimNodeStruct, "Additive") as StructPropertyData;
            if (Base is null || Additive is null) throw new Exception("Failed to get Base/Additive PoseLinks");

            StructPropertyData BaseNodeStruct = GetNodeStructFromPoseLink(Base);
            StructPropertyData AdditiveNodeStruct = GetNodeStructFromPoseLink(Additive);

            AddInputNode("Base", ConvertStructToAnimNode(BaseNodeStruct));
            AddInputNode("Additive", ConvertStructToAnimNode(AdditiveNodeStruct));
        }
    }

    public class AnimNode_AssetPlayerBase : AnimNode_Base
    {
        public AnimNode_AssetPlayerBase(StructPropertyData AnimNodeStruct)
            : base(AnimNodeStruct.Name, AnimNodeStruct.StructType)
        { }
    }

    public class AnimNode_BlendSpacePlayer : AnimNode_AssetPlayerBase
    {
        public string BlendspacePackageName { get; private set; }

        public AnimNode_BlendSpacePlayer(StructPropertyData AnimNodeStruct)
            : base(AnimNodeStruct)
        {
            NodeGraphName = "Blendspace";

            ObjectPropertyData? BlendSpace = Program.FindPropertyInStruct(AnimNodeStruct, "BlendSpace") as ObjectPropertyData;
            if (BlendSpace is null) throw new Exception();

            if (!BlendSpace.IsNull())
                BlendspacePackageName =
                    BlendSpace.ToImport(Program.Asset).OuterIndex.ToImport(Program.Asset).ObjectName.ToString();
            else
                BlendspacePackageName = string.Empty;
        }

        public override string GetNodeInfo()
        {
            return BlendspacePackageName != string.Empty ? $"{NodeGraphName} \"{BlendspacePackageName}\" ({NodeName})"
                : $"{NodeGraphName} ({NodeName})";
        }
    }

    public class AnimNode_AimOffsetLookAt : AnimNode_BlendSpacePlayer
    {
        public AnimNode_AimOffsetLookAt(StructPropertyData AnimNodeStruct)
            : base(AnimNodeStruct)
        {
            NodeGraphName = "LookAt AimOffset";

            StructPropertyData? BasePose = Program.FindPropertyInStruct(AnimNodeStruct, "BasePose") as StructPropertyData;
            if (BasePose is null) throw new Exception("Failed to get BasePose PoseLink");

            StructPropertyData BasePoseNodeStruct = GetNodeStructFromPoseLink(BasePose);
            AddInputNode("BasePose", ConvertStructToAnimNode(BasePoseNodeStruct));
        }
    }

    public class AnimNode_RotationOffsetBlendSpace : AnimNode_BlendSpacePlayer
    {
        public AnimNode_RotationOffsetBlendSpace(StructPropertyData AnimNodeStruct)
            : base(AnimNodeStruct)
        {
            NodeGraphName = "AimOffset";

            StructPropertyData? BasePose = Program.FindPropertyInStruct(AnimNodeStruct, "BasePose") as StructPropertyData;
            if (BasePose is null) throw new Exception("Failed to get BasePose PoseLink");

            StructPropertyData BasePoseNodeStruct = GetNodeStructFromPoseLink(BasePose);
            AddInputNode("BasePose", ConvertStructToAnimNode(BasePoseNodeStruct));
        }
    }

    public class AnimNode_SequenceEvaluator : AnimNode_AssetPlayerBase
    {
        public string AnimSequencePackageName { get; private set; }

        public AnimNode_SequenceEvaluator(StructPropertyData AnimNodeStruct)
            : base(AnimNodeStruct)
        {
            NodeGraphName = "Play Sequence";

            ObjectPropertyData? Sequence = Program.FindPropertyInStruct(AnimNodeStruct, "Sequence") as ObjectPropertyData;
            if (Sequence is null) throw new Exception();

            if (!Sequence.IsNull())
                AnimSequencePackageName =
                    Sequence.ToImport(Program.Asset).OuterIndex.ToImport(Program.Asset).ObjectName.ToString();
            else
                AnimSequencePackageName = string.Empty;
        }

        public override string GetNodeInfo()
        {
            return AnimSequencePackageName != string.Empty ? $"{NodeGraphName} \"{AnimSequencePackageName}\" ({NodeName})"
                : $"{NodeGraphName} ({NodeName})";
        }
    }

    public class AnimNode_SequencePlayer : AnimNode_AssetPlayerBase
    {
        public string AnimSequencePackageName {  get; private set; }

        public AnimNode_SequencePlayer(StructPropertyData AnimNodeStruct)
            : base(AnimNodeStruct)
        {
            NodeGraphName = "Play Sequence";

            ObjectPropertyData? Sequence = Program.FindPropertyInStruct(AnimNodeStruct, "Sequence") as ObjectPropertyData;
            if (Sequence is null) throw new Exception();

            if (!Sequence.IsNull())
                AnimSequencePackageName =
                    Sequence.ToImport(Program.Asset).OuterIndex.ToImport(Program.Asset).ObjectName.ToString();
            else
                AnimSequencePackageName = string.Empty;
        }

        public override string GetNodeInfo()
        {
            return AnimSequencePackageName != string.Empty ? $"{NodeGraphName} \"{AnimSequencePackageName}\" ({NodeName})" 
                : $"{NodeGraphName} ({NodeName})";
        }
    }

    public class AnimNode_BlendListBase : AnimNode_Base
    {
        public AnimNode_BlendListBase(StructPropertyData AnimNodeStruct)
            : base(AnimNodeStruct.Name, AnimNodeStruct.StructType)
        {
            ArrayPropertyData? BlendPose = Program.FindPropertyInStruct(AnimNodeStruct, "BlendPose") as ArrayPropertyData;
            if (BlendPose is null) throw new Exception("Failed to get BlendPose PoseLinks");

            if (AnimNodeStruct.StructType.ToString() != "AnimNode_BlendListByBool")
            {
                int Index = 0;
                foreach (StructPropertyData BlendPose_PoseLink in BlendPose.Value.Cast<StructPropertyData>())
                {
                    StructPropertyData BlendPoseNodeStruct = GetNodeStructFromPoseLink(BlendPose_PoseLink);
                    AddInputNode(Index.ToString(), ConvertStructToAnimNode(BlendPoseNodeStruct));

                    Index++;
                }
            }
        }
    }

    public class AnimNode_BlendListByBool : AnimNode_BlendListBase
    {
        public AnimNode_BlendListByBool(StructPropertyData AnimNodeStruct)
            : base(AnimNodeStruct)
        {
            NodeGraphName = "Blend Poses by bool";

            ArrayPropertyData? BlendPose = Program.FindPropertyInStruct(AnimNodeStruct, "BlendPose") as ArrayPropertyData;
            if (BlendPose is null) throw new Exception("Failed to get BlendPose PoseLinks");

            /** True Pose */
            StructPropertyData TruePoseNodeStruct = GetNodeStructFromPoseLink( (StructPropertyData)BlendPose.Value[0] );
            AddInputNode("True", ConvertStructToAnimNode(TruePoseNodeStruct));

            /** False Pose */
            StructPropertyData FalsePoseNodeStruct = GetNodeStructFromPoseLink( (StructPropertyData)BlendPose.Value[1] );
            AddInputNode("False", ConvertStructToAnimNode(FalsePoseNodeStruct));
        }
    }

    public class AnimNode_BlendListByEnum : AnimNode_BlendListBase
    {
        public AnimNode_BlendListByEnum(StructPropertyData AnimNodeStruct)
            : base(AnimNodeStruct)
        {
            NodeGraphName = "Blend Poses by enum";
        }
    }

    public class AnimNode_BlendListByInt : AnimNode_BlendListBase
    {
        public AnimNode_BlendListByInt(StructPropertyData AnimNodeStruct)
            : base(AnimNodeStruct)
        {
            NodeGraphName = "Blend Poses by int";
        }
    }

    public class AnimNode_ConvertComponentToLocalSpace : AnimNode_Base
    {
        public AnimNode_ConvertComponentToLocalSpace(StructPropertyData AnimNodeStruct)
            : base(AnimNodeStruct.Name, AnimNodeStruct.StructType)
        {
            NodeGraphName = "Component To Local";

            StructPropertyData? ComponentPose = Program.FindPropertyInStruct(AnimNodeStruct, "ComponentPose") as StructPropertyData;
            if (ComponentPose is null) throw new Exception("Failed to get ComponentPose PoseLink");

            StructPropertyData ComponentPoseNodeStruct = GetNodeStructFromPoseLink(ComponentPose);
            AddInputNode("ComponentPose", ConvertStructToAnimNode(ComponentPoseNodeStruct));
        }
    }

    public class AnimNode_ConvertLocalToComponentSpace : AnimNode_Base
    {
        public AnimNode_ConvertLocalToComponentSpace(StructPropertyData AnimNodeStruct)
            : base(AnimNodeStruct.Name, AnimNodeStruct.StructType)
        {
            NodeGraphName = "Local To Component";

            StructPropertyData? LocalPose = Program.FindPropertyInStruct(AnimNodeStruct, "LocalPose") as StructPropertyData;
            if (LocalPose is null) throw new Exception("Failed to get LocalPose PoseLink");

            StructPropertyData LocalPoseNodeStruct = GetNodeStructFromPoseLink(LocalPose);
            AddInputNode("LocalPose", ConvertStructToAnimNode(LocalPoseNodeStruct));
        }
    }

    public class AnimNode_CustomProperty : AnimNode_Base
    {
        public AnimNode_CustomProperty(StructPropertyData AnimNodeStruct)
            : base(AnimNodeStruct.Name, AnimNodeStruct.StructType)
        {
            ObjectPropertyData? TargetInstance = Program.FindPropertyInStruct(AnimNodeStruct, "TargetInstance") as ObjectPropertyData;
            if (TargetInstance is null) throw new Exception();
        }
    }

    public class AnimNode_LinkedAnimGraph : AnimNode_CustomProperty
    {
        public string InstanceClassPackageName {  get; private set; }

        public AnimNode_LinkedAnimGraph(StructPropertyData AnimNodeStruct)
            : base(AnimNodeStruct)
        {
            NodeGraphName = "Linked Anim Graph";

            ArrayPropertyData? InputPoses = Program.FindPropertyInStruct(AnimNodeStruct, "InputPoses") as ArrayPropertyData;
            if (InputPoses is null) throw new Exception("Failed to get InputPoses PoseLinks");

            ArrayPropertyData? InputPoseNames = Program.FindPropertyInStruct(AnimNodeStruct, "InputPoseNames") as ArrayPropertyData;
            if (InputPoseNames is null) throw new Exception("Failed to get InputPoseNames");

            int Index = 0;
            foreach (StructPropertyData InputPose_PoseLink in InputPoses.Value.Cast<StructPropertyData>())
            {
                StructPropertyData InputPoseNodeStruct = GetNodeStructFromPoseLink(InputPose_PoseLink);
                NamePropertyData InputPoseName = (NamePropertyData)InputPoseNames.Value[Index];

                AddInputNode(InputPoseName.ToString(), ConvertStructToAnimNode(InputPoseNodeStruct));

                Index++;
            }

            ObjectPropertyData? InstanceClass = Program.FindPropertyInStruct(AnimNodeStruct, "InstanceClass") as ObjectPropertyData;
            if (InstanceClass is null) throw new Exception();

            /** LinkedAnimLayer might not have InstanceClass set */
            if (!InstanceClass.IsNull())
                InstanceClassPackageName = InstanceClass.ToImport(Program.Asset).OuterIndex.ToImport(Program.Asset).ObjectName.ToString();
            else
                InstanceClassPackageName = "None";
        }

        public override string GetNodeInfo()
        {
            return $"{NodeGraphName} \"{InstanceClassPackageName}\" ({NodeName})";
        }
    }

    public class AnimNode_LinkedAnimLayer : AnimNode_LinkedAnimGraph
    {
        public AnimNode_LinkedAnimLayer(StructPropertyData AnimNodeStruct)
           : base(AnimNodeStruct)
        {
            NodeGraphName = "Linked Anim Layer";
        }
    }

    public class AnimNode_LayeredBoneBlend : AnimNode_Base
    {
        public AnimNode_LayeredBoneBlend(StructPropertyData AnimNodeStruct)
            : base(AnimNodeStruct.Name, AnimNodeStruct.StructType)
        {
            NodeGraphName = "Layered blend per bone";

            /** Base Pose */
            StructPropertyData? BasePose = Program.FindPropertyInStruct(AnimNodeStruct, "BasePose") as StructPropertyData;
            if (BasePose is null) throw new Exception("Failed to get BasePose PoseLink");

            StructPropertyData BasePoseNodeStruct = GetNodeStructFromPoseLink(BasePose);
            AddInputNode("BasePose", ConvertStructToAnimNode(BasePoseNodeStruct));

            /** Blend Poses */
            ArrayPropertyData? BlendPoses = Program.FindPropertyInStruct(AnimNodeStruct, "BlendPoses") as ArrayPropertyData;
            if (BlendPoses is null) throw new Exception("Failed to get BlendPoses PoseLinks");

            int Index = 0;
            foreach (StructPropertyData BlendPose_PoseLink in BlendPoses.Value.Cast<StructPropertyData>())
            {
                StructPropertyData BlendPose_PoseLinkNodeStruct = GetNodeStructFromPoseLink(BlendPose_PoseLink);
                AddInputNode("BlendPose " + Index.ToString(), ConvertStructToAnimNode(BlendPose_PoseLinkNodeStruct));

                Index++;
            }
        }
    }

    public class AnimNode_LinkedInputPose : AnimNode_Base
    {
        public string Name { get; private set; }

        public AnimNode_LinkedInputPose(StructPropertyData AnimNodeStruct)
            : base(AnimNodeStruct.Name, AnimNodeStruct.StructType)
        {
            NodeGraphName = "Input Pose";

            StructPropertyData? InputPose = Program.FindPropertyInStruct(AnimNodeStruct, "InputPose") as StructPropertyData;
            if (InputPose is null) throw new Exception("Failed to get InputPose PoseLink");
            IntPropertyData LinkID = Program.FindPropertyInStruct<IntPropertyData>(InputPose, "LinkID");

            if (LinkID.Value != -1)
            {
                StructPropertyData InputPoseNodeStruct = GetNodeStructFromPoseLink(InputPose);
                AddInputNode("InputPose", ConvertStructToAnimNode(InputPoseNodeStruct));
            }

            Name = Program.FindPropertyInStruct<NamePropertyData>(AnimNodeStruct, "Name").Value.ToString();
        }

        public override string GetNodeInfo()
        {
            return $"{NodeGraphName} \"{Name}\" ({NodeName})";
        }
    }

    public class AnimNode_MeshSpaceRefPose : AnimNode_Base
    {
        public AnimNode_MeshSpaceRefPose(StructPropertyData AnimNodeStruct)
            : base(AnimNodeStruct.Name, AnimNodeStruct.StructType)
        {
            NodeGraphName = "Mesh Space Ref Pose";
        }
    }

    public class AnimNode_MultiWayBlend : AnimNode_Base
    {
        public AnimNode_MultiWayBlend(StructPropertyData AnimNodeStruct)
            : base(AnimNodeStruct.Name, AnimNodeStruct.StructType)
        {
            NodeGraphName = "Blend Multi";

            ArrayPropertyData? Poses = Program.FindPropertyInStruct(AnimNodeStruct, "Poses") as ArrayPropertyData;
            if (Poses is null) throw new Exception("Failed to get Poses PoseLinks");

            int Index = 0;
            foreach (StructPropertyData Pose_PoseLink in Poses.Value.Cast<StructPropertyData>())
            {
                StructPropertyData PoseNodeStruct = GetNodeStructFromPoseLink(Pose_PoseLink);
                AddInputNode(Index.ToString(), ConvertStructToAnimNode(PoseNodeStruct));

                Index++;
            }
        }
    }

    public class AnimNode_RandomPlayer : AnimNode_Base
    {
        public AnimNode_RandomPlayer(StructPropertyData AnimNodeStruct)
            : base(AnimNodeStruct.Name, AnimNodeStruct.StructType)
        {
            NodeGraphName = "Random Sequence Player";
        }
    }

    public class AnimNode_RefPose : AnimNode_Base
    {
        public AnimNode_RefPose(StructPropertyData AnimNodeStruct)
            : base(AnimNodeStruct.Name, AnimNodeStruct.StructType)
        {
            NodeGraphName = "Local Space Ref Pose";
        }
    }

    public class AnimNode_Root : AnimNode_Base
    {
        public AnimNode_Root(StructPropertyData AnimNodeStruct)
            : base(AnimNodeStruct.Name, AnimNodeStruct.StructType)
        {
            NodeGraphName = "Output Pose";

            StructPropertyData? Result = Program.FindPropertyInStruct(AnimNodeStruct, "Result") as StructPropertyData;
            if (Result is null) throw new Exception("Failed to get Result PoseLink");

            StructPropertyData ResultNodeStruct = GetNodeStructFromPoseLink(Result);
            AddInputNode("Result", ConvertStructToAnimNode(ResultNodeStruct));
        }
    }

    public class AnimNode_StateResult : AnimNode_Root
    {
        public AnimNode_StateResult(StructPropertyData AnimNodeStruct)
            : base(AnimNodeStruct)
        {
            NodeGraphName = "Output Animation Pose";
        }
    }

    public class AnimNode_SaveCachedPose : AnimNode_Base
    {
        public string CachePoseName {  get; private set; }

        public AnimNode_SaveCachedPose(StructPropertyData AnimNodeStruct)
            : base(AnimNodeStruct.Name, AnimNodeStruct.StructType)
        {
            NodeGraphName = "Save Cached Pose";

            NamePropertyData CachePoseNameData = Program.FindPropertyInStruct<NamePropertyData>(AnimNodeStruct, "CachePoseName");
            CachePoseName = CachePoseNameData.Value.ToString();

            StructPropertyData? Pose = Program.FindPropertyInStruct(AnimNodeStruct, "Pose") as StructPropertyData;
            if (Pose is null) throw new Exception("Failed to get Pose PoseLink");

            StructPropertyData PoseNodeStruct = GetNodeStructFromPoseLink(Pose);
            AddInputNode("Pose", ConvertStructToAnimNode(PoseNodeStruct));
        }

        public override string GetNodeInfo()
        {
            return $"{NodeGraphName} \"{CachePoseName}\" ({NodeName})";
        }
    }

    public class AnimNode_SkeletalControlBase : AnimNode_Base
    {
        public AnimNode_SkeletalControlBase(StructPropertyData AnimNodeStruct)
            : base(AnimNodeStruct.Name, AnimNodeStruct.StructType)
        {
            StructPropertyData? ComponentPose = Program.FindPropertyInStruct(AnimNodeStruct, "ComponentPose") as StructPropertyData;
            if (ComponentPose is null) throw new Exception("Failed to get ComponentPose PoseLink");

            StructPropertyData ComponentPoseNodeStruct = GetNodeStructFromPoseLink(ComponentPose);
            AddInputNode("ComponentPose", ConvertStructToAnimNode(ComponentPoseNodeStruct));
        }
    }

    public class AnimNode_AnimDynamics : AnimNode_SkeletalControlBase
    {
        public AnimNode_AnimDynamics(StructPropertyData AnimNodeStruct)
            : base(AnimNodeStruct)
        {
            NodeGraphName = "Anim Dynamics";            
        }
    }

    public class AnimNode_Fabrik : AnimNode_SkeletalControlBase
    {
        public AnimNode_Fabrik(StructPropertyData AnimNodeStruct)
            : base(AnimNodeStruct)
        {
            NodeGraphName = "FABRIK";
        }
    }

    public class AnimNode_ModifyBone : AnimNode_SkeletalControlBase
    {
        public AnimNode_ModifyBone(StructPropertyData AnimNodeStruct)
            : base(AnimNodeStruct)
        {
            NodeGraphName = "Transform (Modify) Bone";
        }
    }

    public class AnimNode_SpringBone : AnimNode_SkeletalControlBase
    {
        public AnimNode_SpringBone(StructPropertyData AnimNodeStruct)
            : base(AnimNodeStruct)
        {
            NodeGraphName = "Spring controller";
        }
    }

    public class AnimNode_TwoBoneIK : AnimNode_SkeletalControlBase
    {
        public AnimNode_TwoBoneIK(StructPropertyData AnimNodeStruct)
            : base(AnimNodeStruct)
        {
            NodeGraphName = "Two Bone IK";
        }
    }

    public class AnimNode_Slot : AnimNode_Base
    {
        public string SlotName {  get; private set; }

        public AnimNode_Slot(StructPropertyData AnimNodeStruct)
            : base(AnimNodeStruct.Name, AnimNodeStruct.StructType)
        {
            NodeGraphName = "Slot";

            StructPropertyData? Source = Program.FindPropertyInStruct(AnimNodeStruct, "Source") as StructPropertyData;
            if (Source is null) throw new Exception("Failed to get Source PoseLink");

            StructPropertyData SourceNodeStruct = GetNodeStructFromPoseLink(Source);
            AddInputNode("Source", ConvertStructToAnimNode(SourceNodeStruct));

            NamePropertyData SlotNameData = Program.FindPropertyInStruct<NamePropertyData>(AnimNodeStruct, "SlotName");
            SlotName = SlotNameData.Value.ToString();
        }

        public override string GetNodeInfo()
        {
            return $"{NodeGraphName} \"{SlotName}\" ({NodeName})";
        }
    }

    public class AnimNode_StateMachine : AnimNode_Base
    {
        public string StateMachineName { get; private set; }

        public AnimNode_StateMachine(StructPropertyData AnimNodeStruct)
            : base(AnimNodeStruct.Name, AnimNodeStruct.StructType)
        {
            NodeGraphName = "State Machine";

            /** Get StateMachine */
            IntPropertyData StateMachineIndexInClassData = 
                Program.FindPropertyInStruct<IntPropertyData>(AnimNodeStruct, "StateMachineIndexInClass");

            StateMachineName = Program.StateMachines[StateMachineIndexInClassData.Value].MachineName;
        }

        public override string GetNodeInfo()
        {
            return $"{NodeGraphName} \"{StateMachineName}\" ({NodeName})";
        }
    }

    public class AnimNode_TransitionResult : AnimNode_Base
    {
        public AnimNode_TransitionResult(StructPropertyData AnimNodeStruct)
            : base(AnimNodeStruct.Name, AnimNodeStruct.StructType)
        {
            NodeGraphName = "Transition Result";
        }
    }

    public class AnimNode_TwoWayBlend : AnimNode_Base
    {
        public AnimNode_TwoWayBlend(StructPropertyData AnimNodeStruct)
            : base(AnimNodeStruct.Name, AnimNodeStruct.StructType)
        {
            NodeGraphName = "Blend";

            StructPropertyData? A = Program.FindPropertyInStruct(AnimNodeStruct, "A") as StructPropertyData;
            StructPropertyData? B = Program.FindPropertyInStruct(AnimNodeStruct, "B") as StructPropertyData;
            if (A is null || B is null) throw new Exception("Failed to get A/B PoseLinks");

            StructPropertyData A_NodeStruct = GetNodeStructFromPoseLink(A);
            StructPropertyData B_NodeStruct = GetNodeStructFromPoseLink(B);

            AddInputNode("A", ConvertStructToAnimNode(A_NodeStruct));
            AddInputNode("B", ConvertStructToAnimNode(B_NodeStruct));
        }
    }

    public class AnimNode_UseCachedPose : AnimNode_Base
    {
        public string CachePoseName { get; private set; }

        public AnimNode_UseCachedPose(StructPropertyData AnimNodeStruct)
            : base(AnimNodeStruct.Name, AnimNodeStruct.StructType)
        {
            NodeGraphName = "Use Cached Pose";

            NamePropertyData CachePoseNameData = Program.FindPropertyInStruct<NamePropertyData>(AnimNodeStruct, "CachePoseName");
            CachePoseName = CachePoseNameData.Value.ToString();

            /*
            StructPropertyData? LinkToCachingNode = Program.FindPropertyInStruct(AnimNodeStruct, "LinkToCachingNode") as StructPropertyData;
            if (LinkToCachingNode is null) throw new Exception("Failed to get LinkToCachingNode PoseLink");

            StructPropertyData LinkToCachingNode_NodeStruct = GetNodeStructFromPoseLink(LinkToCachingNode);
            AddInputNode("LinkToCachingNode", ConvertStructToAnimNode(LinkToCachingNode_NodeStruct));
            */
        }

        public override string GetNodeInfo()
        {
            return $"{NodeGraphName} \"{CachePoseName}\" ({NodeName})";
        }
    }
}
