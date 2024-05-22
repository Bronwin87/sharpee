using DataStore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TextService;

namespace StandardLibrary
{
    public class Story
    {
        private TitleCard titleCard;
        private World world;
        private Map map;
        private string playerCharacterId;
        private ITextService textService;

        public Story(TitleCard titleCard, World world, Map map, ITextService textService)
        {
            this.world = world;
            this.map = map;
            this.textService = textService;
            this.titleCard = titleCard;
            this.playerCharacterId = ""; // default for now
        }

        /// <summary>
        /// Starts the story (turn loop)
        /// </summary>
        /// <param name="prologue"></param>
        public void Begin(string prologue)
        {
            EmitStoryMetadata();
            EmitPrologue(prologue);
        }

        private void TurnLoop()
        {

        }

        public Story AddThing(string locationId, string slashSeparatedName, string description, params (string name, object value)[] properties)
        {
            if (!map.RoomExists(locationId))
            {
                throw new ArgumentException($"Room '{locationId}' does not exist in the map.");
            }

            string objectId = GenerateObjectId(slashSeparatedName);
            string[] names = slashSeparatedName.Split('/');
            string name = names[0];
            string[] synonyms = names.Skip(1).ToArray();

            world.AddNode(objectId, description);
            SetNodeName(objectId, name, synonyms);
            PlaceObjectInLocation(objectId, locationId);

            foreach (var property in properties)
            {
                world.SetNodeProperty(objectId, property.name, property.value);
            }

            return this;
        }

        public Story AddPlayerCharacter(string characterId, string name, string description, string locationId, string race, string gender, string[] pronouns, params string[] synonyms)
        {
            AddThing(locationId, $"{name}/{string.Join("/", synonyms)}", description);
            SetCharacterProperties(characterId, race, gender, pronouns);
            world.SetNodeProperty(characterId, "isPlayer", true);
            playerCharacterId = characterId;
            return this;
        }

        public Story AddNonPlayerCharacter(string characterId, string name, string description, string locationId, string race, string gender, string[] pronouns, params string[] synonyms)
        {
            AddThing(locationId, $"{name}/{string.Join("/", synonyms)}", description);
            SetCharacterProperties(characterId, race, gender, pronouns);
            world.SetNodeProperty(characterId, "isNPC", true);
            return this;
        }

        public Story PlaceCharacterInLocation(string characterId, string locationId)
        {
            PlaceObjectInLocation(characterId, locationId);
            return this;
        }

        public string GetPlayerCharacterId()
        {
            return playerCharacterId;
        }

        public Story CharacterTakeObject(string characterId, string objectId)
        {
            if (IsObjectAccessible(characterId, objectId))
            {
                var currentLocationId = GetCharacterLocation(characterId);
                MoveObject(objectId, currentLocationId, characterId);
            }
            return this;
        }

        public Story CharacterDropObject(string characterId, string objectId)
        {
            if (IsObjectHeldByCharacter(characterId, objectId))
            {
                var currentLocationId = GetCharacterLocation(characterId);
                MoveObject(objectId, characterId, currentLocationId);
            }
            return this;
        }

        public Story CharacterPutObjectInContainer(string characterId, string objectId, string containerId)
        {
            if (IsObjectHeldByCharacter(characterId, objectId) && IsContainerAccessible(characterId, containerId))
            {
                PlaceObjectInContainer(objectId, containerId);
            }
            return this;
        }

        public Story CharacterPutObjectOnSupporter(string characterId, string objectId, string supporterId)
        {
            if (IsObjectHeldByCharacter(characterId, objectId) && IsSupporterAccessible(characterId, supporterId))
            {
                PlaceObjectOnSupporter(objectId, supporterId);
            }
            return this;
        }

        public Story CharacterGiveObjectToCharacter(string givingCharacterId, string receivingCharacterId, string objectId)
        {
            if (IsObjectHeldByCharacter(givingCharacterId, objectId) && IsCharacterAccessible(givingCharacterId, receivingCharacterId))
            {
                MoveObject(objectId, givingCharacterId, receivingCharacterId);
            }
            return this;
        }

        public Story MoveObject(string objectId, string currentLocationId, string newLocationId)
        {
            if (!IsObjectScenery(objectId) && !IsObjectImmovable(objectId))
            {
                world.DisconnectNodes(objectId, currentLocationId);
                world.ConnectNodes(objectId, newLocationId, "IN", "HOLDS");
            }
            return this;
        }

        public string GetObjectLocation(string objectId)
        {
            var inEdge = world.Nodes[objectId].Edges.Find(e => e.EdgeType == "IN");
            if (inEdge != null)
            {
                return inEdge.Id2;
            }

            var insideEdge = world.Nodes[objectId].Edges.Find(e => e.EdgeType == "INSIDE");
            if (insideEdge != null)
            {
                return insideEdge.Id2;
            }

            var onEdge = world.Nodes[objectId].Edges.Find(e => e.EdgeType == "ON");
            if (onEdge != null)
            {
                return onEdge.Id2;
            }

            return null;
        }

        private void EmitStoryMetadata()
        {
            var metadata = new
            {
                titleCard.Title,
                titleCard.Authors,
                titleCard.Publisher,
                Version = $"{titleCard.MajorVersion}.{titleCard.MinorVersion}.{titleCard.PatchVersion}",
                titleCard.IFID,
                titleCard.DateCompiled
            };

            EmitJson(metadata);
        }

        private void EmitPrologue(string prologue)
        {
            EmitJson(new { Prologue = prologue });
        }

        private void EmitJson(object data)
        {
            string json = JsonConvert.SerializeObject(data);
            textService.Emit(json);
        }

        private bool IsObjectAccessible(string characterId, string objectId)
        {
            var characterLocationId = GetCharacterLocation(characterId);
            var objectLocationId = GetObjectLocation(objectId);
            return characterLocationId == objectLocationId;
        }

        private bool IsContainerAccessible(string characterId, string containerId)
        {
            var characterLocationId = GetCharacterLocation(characterId);
            var containerLocationId = GetObjectLocation(containerId);
            return characterLocationId == containerLocationId && IsContainerOpen(containerId);
        }

        private bool IsSupporterAccessible(string characterId, string supporterId)
        {
            var characterLocationId = GetCharacterLocation(characterId);
            var supporterLocationId = GetObjectLocation(supporterId);
            return characterLocationId == supporterLocationId;
        }

        private bool IsCharacterAccessible(string characterId, string otherCharacterId)
        {
            var characterLocationId = GetCharacterLocation(characterId);
            var otherCharacterLocationId = GetCharacterLocation(otherCharacterId);
            return characterLocationId == otherCharacterLocationId;
        }

        private bool IsObjectHeldByCharacter(string characterId, string objectId)
        {
            var objectLocationId = GetObjectLocation(objectId);
            return objectLocationId == characterId;
        }

        private string GetCharacterLocation(string characterId)
        {
            var inEdge = world.Nodes[characterId].Edges.Find(e => e.EdgeType == "IN");
            return inEdge?.Id2;
        }

        private bool IsObjectScenery(string objectId)
        {
            var sceneryProperty = world.Nodes[objectId].Properties.Find(p => p.Name == "scenery");
            return sceneryProperty != null;
        }

        private bool IsObjectImmovable(string objectId)
        {
            var isImmovableProperty = world.Nodes[objectId].Properties.Find(p => p.Name == "isImmovable");
            return (bool)(isImmovableProperty?.Value ?? false);
        }

        private bool IsContainerOpen(string containerId)
        {
            var isOpenProperty = world.Nodes[containerId].Properties.Find(p => p.Name == "isOpen");
            return (bool)(isOpenProperty?.Value ?? false);
        }

        private void SetNodeName(string nodeId, string name, params string[] synonyms)
        {
            world.SetNodeProperty(nodeId, "name", name);
            world.SetNodeProperty(nodeId, "synonyms", synonyms);
        }

        private void PlaceObjectInLocation(string objectId, string locationId)
        {
            world.AddEdgeType("IN", "HOLDS");
            world.ConnectNodes(objectId, locationId, "IN", "HOLDS");
        }

        private void PlaceObjectInContainer(string objectId, string containerId)
        {
            world.AddEdgeType("INSIDE", "CONTAINS");
            world.ConnectNodes(objectId, containerId, "INSIDE", "CONTAINS");
        }

        private void PlaceObjectOnSupporter(string objectId, string supporterId)
        {
            world.AddEdgeType("ON", "SUPPORTS");
            world.ConnectNodes(objectId, supporterId, "ON", "SUPPORTS");
        }

        private void SetCharacterProperties(string characterId, string race, string gender, string[] pronouns)
        {
            world.SetNodeProperty(characterId, "race", race);
            world.SetNodeProperty(characterId, "gender", gender);
            world.SetNodeProperty(characterId, "pronouns", pronouns);
        }

        private string GenerateObjectId(string slashSeparatedName)
        {
            return slashSeparatedName.Replace('/', '_');
        }
    }
}
