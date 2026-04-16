import { Input } from "./ui/input";
import { Label } from "./ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "./ui/select";

interface CharacterBasicsProps {
  name: string;
  level: number;
  ancestry: string;
  background: string;
  characterClass: string;
  onNameChange: (name: string) => void;
  onLevelChange: (level: number) => void;
  onAncestryChange: (ancestry: string) => void;
  onBackgroundChange: (background: string) => void;
  onClassChange: (characterClass: string) => void;
}

const ancestries = [
  "Human",
  "Elf",
  "Dwarf",
  "Gnome",
  "Goblin",
  "Halfling",
  "Orc",
  "Leshy",
  "Lizardfolk",
  "Catfolk"
];

const backgrounds = [
  "Acolyte",
  "Acrobat",
  "Animal Whisperer",
  "Artisan",
  "Artist",
  "Barkeep",
  "Bounty Hunter",
  "Charlatan",
  "Criminal",
  "Detective",
  "Emissary",
  "Entertainer",
  "Farmhand",
  "Field Medic",
  "Fortune Teller",
  "Gambler",
  "Guard",
  "Herbalist",
  "Hunter",
  "Laborer",
  "Martial Disciple",
  "Merchant",
  "Miner",
  "Noble",
  "Nomad",
  "Prisoner",
  "Sailor",
  "Scholar",
  "Scout",
  "Street Urchin",
  "Tinker",
  "Warrior"
];

const classes = [
  "Alchemist",
  "Barbarian",
  "Bard",
  "Champion",
  "Cleric",
  "Druid",
  "Fighter",
  "Gunslinger",
  "Inventor",
  "Investigator",
  "Magus",
  "Monk",
  "Oracle",
  "Psychic",
  "Ranger",
  "Rogue",
  "Sorcerer",
  "Summoner",
  "Swashbuckler",
  "Thaumaturge",
  "Witch",
  "Wizard"
];

export function CharacterBasics({
  name,
  level,
  ancestry,
  background,
  characterClass,
  onNameChange,
  onLevelChange,
  onAncestryChange,
  onBackgroundChange,
  onClassChange
}: CharacterBasicsProps) {
  return (
    <div className="space-y-4">
      <div className="grid grid-cols-2 gap-4">
        <div className="space-y-2">
          <Label htmlFor="name">Character Name</Label>
          <Input
            id="name"
            value={name}
            onChange={(e) => onNameChange(e.target.value)}
            placeholder="Enter character name"
          />
        </div>
        <div className="space-y-2">
          <Label htmlFor="level">Level</Label>
          <Input
            id="level"
            type="number"
            min="1"
            max="20"
            value={level}
            onChange={(e) => onLevelChange(parseInt(e.target.value) || 1)}
          />
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div className="space-y-2">
          <Label htmlFor="ancestry">Ancestry</Label>
          <Select value={ancestry} onValueChange={onAncestryChange}>
            <SelectTrigger id="ancestry">
              <SelectValue placeholder="Select ancestry" />
            </SelectTrigger>
            <SelectContent>
              {ancestries.map((a) => (
                <SelectItem key={a} value={a}>
                  {a}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>

        <div className="space-y-2">
          <Label htmlFor="background">Background</Label>
          <Select value={background} onValueChange={onBackgroundChange}>
            <SelectTrigger id="background">
              <SelectValue placeholder="Select background" />
            </SelectTrigger>
            <SelectContent>
              {backgrounds.map((b) => (
                <SelectItem key={b} value={b}>
                  {b}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>

        <div className="space-y-2">
          <Label htmlFor="class">Class</Label>
          <Select value={characterClass} onValueChange={onClassChange}>
            <SelectTrigger id="class">
              <SelectValue placeholder="Select class" />
            </SelectTrigger>
            <SelectContent>
              {classes.map((c) => (
                <SelectItem key={c} value={c}>
                  {c}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
      </div>
    </div>
  );
}
