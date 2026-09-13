using System;
using Items;
using Progression;
using Recipes;
using UnityEngine;
using UnityEngine.Localization;

namespace Research
{
	[CreateAssetMenu(fileName = "Research Sample", menuName = "Expedition/Research/Sample")]
	[UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Survival")]
	public class ResearchSampleConfig : ScriptableObject, IResearchDefinition, Peleng.IPelengMetadata
	{
		[field: SerializeField]
		public MilestoneConfig Milestone { get; private set; }

		[field: SerializeField]
		public MilestoneConfig Prerequisite { get; private set; }

		[field: SerializeField]
		public ItemConfig SourceItem { get; private set; }

		[field: SerializeField, Min(1f)]
		public float Duration { get; private set; } = 120f;
		[field: SerializeField]
		public LocalizedString Description { get; private set; }

		[field: SerializeField]
		public LocalizedString NextStep { get; private set; }

		[field: SerializeField]
		public LocalizedString Method { get; private set; }

		[field: SerializeField]
		public string MediaExtension { get; private set; }

		public LocalizedString PelengName => Milestone.Title;
		public LocalizedString PelengDescription => Description;
		public Sprite PelengIcon => Milestone.Icon;
		public UnityEngine.AddressableAssets.AssetReferenceSprite PelengIconReference => null;
		public string ResearchId => Milestone.Id;
		public bool IsCompleted(IProgressionState progression, IRecipeDiscovery recipes)
		{
			return progression.IsCompleted(Milestone.Id);
		}

		public bool IsUnlocked(IProgressionState progression)
		{
			return !Prerequisite || progression.IsCompleted(Prerequisite.Id);
		}

		public void Complete(IProgressionState progression, IRecipeDiscovery recipes)
		{
			if (!progression.TryComplete(Milestone.Id))
			{
				throw new InvalidOperationException($"Research milestone '{Milestone.Id}' could not complete.");
			}
		}

		public virtual bool RequiresAnalysis => true;
		public virtual string SubmitKey => "Samples.Submit";
		public virtual bool CanSubmit(IResearchProgress session) => session.IsCollected(ResearchId);
		public virtual bool AcceptsRecipient(string recipientId) => !string.IsNullOrEmpty(recipientId);
		public virtual void Validate()
		{
			if (!Milestone || Duration <= 0f || Milestone.HasAutomaticRequirements)
			{
				throw new InvalidOperationException($"Research sample '{name}' requires a manual milestone and positive duration.");
			}
		}
	}
}
