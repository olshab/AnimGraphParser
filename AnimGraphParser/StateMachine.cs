using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;

namespace AnimGraphParser
{
    public class BakedStateExitTransition
    {
        public int CanTakeDelegateIndex { get; private set; }
        public int TransitionIndex { get; private set; }
        public bool bAutomaticRemainingTimeRule { get; private set; }

        public BakedStateExitTransition(StructPropertyData ExitTransitionStruct)
        {
            IntPropertyData CanTakeDelegateIndexData =
                Program.FindPropertyInStruct<IntPropertyData>(ExitTransitionStruct, "CanTakeDelegateIndex");
            IntPropertyData TransitionIndexData =
                Program.FindPropertyInStruct<IntPropertyData>(ExitTransitionStruct, "TransitionIndex");
            BoolPropertyData bAutomaticRemainingTimeRuleData =
                Program.FindPropertyInStruct<BoolPropertyData>(ExitTransitionStruct, "bAutomaticRemainingTimeRule");

            CanTakeDelegateIndex = CanTakeDelegateIndexData.Value;
            TransitionIndex = TransitionIndexData.Value;
            bAutomaticRemainingTimeRule = bAutomaticRemainingTimeRuleData.Value;
        }
    }

    public class BakedAnimationState
    {
        public string StateName { get; private set; }
        public List<BakedStateExitTransition> Transitions { get; private set; }
        public int StateRootNodeIndex { get; private set; }

        public BakedAnimationState(StructPropertyData StateStruct)
        {
            NamePropertyData StateNameData =
                Program.FindPropertyInStruct<NamePropertyData>(StateStruct, "StateName");
            IntPropertyData StateRootNodeIndexData =
                Program.FindPropertyInStruct<IntPropertyData>(StateStruct, "StateRootNodeIndex");

            StateName = StateNameData.Value.ToString();
            StateRootNodeIndex = StateRootNodeIndexData.Value;
            Transitions = new List<BakedStateExitTransition>();

            ArrayPropertyData TransitionsData =
                Program.FindPropertyInStruct<ArrayPropertyData>(StateStruct, "Transitions");

            foreach (StructPropertyData BakedStateExitTransition in TransitionsData.Value.Cast<StructPropertyData>())
                Transitions.Add(new BakedStateExitTransition(BakedStateExitTransition));            
        }
    }

    public class AnimationTransitionBetweenStates
    {
        public int PreviousState { get; private set; }
        public int NextState { get; private set; }

        public AnimationTransitionBetweenStates(StructPropertyData TransitionStruct)
        {
            IntPropertyData PreviousStateData =
               Program.FindPropertyInStruct<IntPropertyData>(TransitionStruct, "PreviousState");
            IntPropertyData NextStateData =
                Program.FindPropertyInStruct<IntPropertyData>(TransitionStruct, "NextState");

            PreviousState = PreviousStateData.Value;
            NextState = NextStateData.Value;
        }
    }

    public class BakedAnimationStateMachine
    {
        public string MachineName { get; private set; }
        public List<BakedAnimationState> States { get; private set; }
        public List<AnimationTransitionBetweenStates> Transitions { get; private set; }

        public BakedAnimationStateMachine(StructPropertyData StateMachineStruct)
        {
            NamePropertyData MachineNameData =
               Program.FindPropertyInStruct<NamePropertyData>(StateMachineStruct, "MachineName");
            ArrayPropertyData StatesData =
                Program.FindPropertyInStruct<ArrayPropertyData>(StateMachineStruct, "States");
            ArrayPropertyData TransitionsData =
                Program.FindPropertyInStruct<ArrayPropertyData>(StateMachineStruct, "Transitions");

            MachineName = MachineNameData.Value.ToString();

            States = new List<BakedAnimationState>();
            foreach (StructPropertyData State in StatesData.Value.Cast<StructPropertyData>())
                States.Add(new BakedAnimationState(State));

            Transitions = new List<AnimationTransitionBetweenStates>();
            foreach (StructPropertyData Transition in TransitionsData.Value.Cast<StructPropertyData>())
                Transitions.Add(new AnimationTransitionBetweenStates(Transition));
        }
    }
}
