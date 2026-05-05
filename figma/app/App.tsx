import { useState } from "react";
import { Menu, X, LogOut, Users } from "lucide-react";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "./components/ui/tabs";
import { ScrollArea } from "./components/ui/scroll-area";
import { Button } from "./components/ui/button";
import { Sidebar } from "./components/Sidebar";
import { CharacterHeader } from "./components/CharacterHeader";
import { StatsPanel } from "./components/StatsPanel";
import { SkillsList } from "./components/SkillsList";
import { WeaponsTab } from "./components/WeaponsTab";
import { FeatsTab } from "./components/FeatsTab";
import { ImageWithFallback } from "./components/figma/ImageWithFallback";
import logoImage from "figma:asset/7aff5a0e20213fe4099810b82abb6121ba941f67.png";

const initialSkills = [
  { name: "Acrobatics", ability: "DEX", proficiency: "trained" as const, modifier: 1 },
  { name: "Arcana", ability: "INT", proficiency: "untrained" as const, modifier: 0 },
  { name: "Athletics", ability: "STR", proficiency: "trained" as const, modifier: 3 },
  { name: "Crafting", ability: "INT", proficiency: "untrained" as const, modifier: 0 },
  { name: "Deception", ability: "CHA", proficiency: "untrained" as const, modifier: 0 },
  { name: "Diplomacy", ability: "CHA", proficiency: "trained" as const, modifier: 3 },
  { name: "Intimidation", ability: "CHA", proficiency: "untrained" as const, modifier: 0 },
  { name: "Lore: Alcohol", ability: "INT", proficiency: "trained" as const, modifier: 3 },
  { name: "Medicine", ability: "WIS", proficiency: "untrained" as const, modifier: 0 },
  { name: "Nature", ability: "WIS", proficiency: "untrained" as const, modifier: 0 },
  { name: "Occultism", ability: "INT", proficiency: "untrained" as const, modifier: 0 },
  { name: "Performance", ability: "CHA", proficiency: "untrained" as const, modifier: 0 },
  { name: "Religion", ability: "WIS", proficiency: "untrained" as const, modifier: 0 },
  { name: "Society", ability: "INT", proficiency: "untrained" as const, modifier: 0 },
  { name: "Stealth", ability: "DEX", proficiency: "trained" as const, modifier: 1 },
  { name: "Survival", ability: "WIS", proficiency: "untrained" as const, modifier: 0 },
  { name: "Thievery", ability: "DEX", proficiency: "trained" as const, modifier: 1 }
];

export default function App() {
  const [sidebarOpen, setSidebarOpen] = useState(true);
  const [name, setName] = useState("Unknown Adventurer");
  const [level, setLevel] = useState(1);
  const [ancestry, setAncestry] = useState("Human");
  const [background, setBackground] = useState("Barkeep");
  const [characterClass, setCharacterClass] = useState("Fighter");
  const [activeTab, setActiveTab] = useState("defense");

  const [abilityScores, setAbilityScores] = useState({
    strength: 16,
    dexterity: 12,
    constitution: 14,
    intelligence: 10,
    wisdom: 10,
    charisma: 10
  });

  const [skills, setSkills] = useState(initialSkills);

  const handleSkillChange = (
    index: number,
    proficiency: "untrained" | "trained" | "expert" | "master" | "legendary"
  ) => {
    setSkills((prev) => {
      const updated = [...prev];
      updated[index] = { ...updated[index], proficiency };
      return updated;
    });
  };

  return (
    <div className="h-screen flex flex-col bg-[#f5f1e8] text-[#5c4a3a] overflow-hidden">
      {/* Top Header */}
      <div className="h-10 bg-[#e8e3d6] border-b border-[#c4b8a0] flex items-center px-3 gap-3 shrink-0">
        {/* Logo */}
        <ImageWithFallback
          src={logoImage}
          alt="Roll Call Logo"
          className="h-7 w-auto"
        />

        <Button
          variant="ghost"
          size="sm"
          onClick={() => setSidebarOpen(!sidebarOpen)}
          className="text-[#5c4a3a] hover:text-[#3d2f24] hover:bg-[#d8d0bf] h-7 px-2"
        >
          {sidebarOpen ? <X className="w-4 h-4" /> : <Menu className="w-4 h-4" />}
          <span className="ml-1 text-xs">Menu</span>
        </Button>
        <div className="text-sm text-[#8b7355]">×</div>
        <div className="text-xs text-[#5c4a3a]">{name} - {characterClass} {level}</div>

        {/* Right side buttons */}
        <div className="ml-auto flex items-center gap-2">
          <Button
            variant="ghost"
            size="sm"
            className="text-[#5c4a3a] hover:text-[#3d2f24] hover:bg-[#d8d0bf] h-7 px-2 gap-1.5"
          >
            <Users className="w-3.5 h-3.5" />
            <span className="text-xs">Join DM</span>
          </Button>
          <Button
            variant="ghost"
            size="sm"
            className="text-[#5c4a3a] hover:text-[#3d2f24] hover:bg-[#d8d0bf] h-7 px-2 gap-1.5"
          >
            <LogOut className="w-3.5 h-3.5" />
            <span className="text-xs">Log Out</span>
          </Button>
        </div>
      </div>

      {/* Main Content */}
      <div className="flex flex-1 overflow-hidden">
        {/* Sidebar */}
        {sidebarOpen && (
          <div className="w-48 bg-[#ede9dd] border-r border-[#c4b8a0] shrink-0">
            <ScrollArea className="h-full">
              <Sidebar
                ancestry={ancestry}
                background={background}
                characterClass={characterClass}
                level={level}
                onAncestryChange={setAncestry}
                onBackgroundChange={setBackground}
                onClassChange={setCharacterClass}
              />
            </ScrollArea>
          </div>
        )}

        {/* Center Content - Stats and Skills */}
        <div className="w-80 bg-[#ede9dd] border-r border-[#c4b8a0] shrink-0 flex flex-col">
          {/* Character Mini Header */}
          <div className="border-b border-[#c4b8a0] p-3 shrink-0">
            <div className="flex items-start gap-3">
              <div className="flex gap-2 text-xs">
                <button className="text-[#8b7355] hover:text-[#5c4a3a]">Hide</button>
                <button className="text-[#8b7355] hover:text-[#5c4a3a]">Path</button>
              </div>
              <div className="flex-1">
                <div className="flex gap-3 text-xs mb-2">
                  <div>
                    <span className="text-[#8b7355]">Level</span>
                    <span className="ml-2">{level}</span>
                  </div>
                  <div>
                    <span className="text-[#8b7355]">XP</span>
                    <span className="ml-2">0</span>
                  </div>
                </div>
                <div className="text-xs text-[#5c4a3a] mb-2">{name}</div>
                <div className="flex gap-4 text-xs">
                  <div>
                    <span className="text-red-700">SIZE</span>
                    <span className="ml-1 text-[#5c4a3a]">Medium</span>
                  </div>
                  <div>
                    <span className="text-red-700">SPEED</span>
                    <span className="ml-1 text-[#5c4a3a]">25ft</span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <ScrollArea className="flex-1">
            <div className="p-3">
              <StatsPanel abilityScores={abilityScores} />
              <SkillsList skills={skills} onSkillChange={handleSkillChange} />
            </div>
          </ScrollArea>
        </div>

        {/* Right Panel - Main Content */}
        <div className="flex-1 flex flex-col overflow-hidden">
          {/* Character Stats Header - Compact */}
          <div className="bg-[#ede9dd] border-b border-[#c4b8a0] p-3 shrink-0">
            <CharacterHeader
              name={name}
              level={level}
              abilityScores={abilityScores}
              onNameChange={setName}
              onAbilityScoreChange={setAbilityScores}
            />
          </div>

          {/* Tabs and Content */}
          <div className="flex-1 overflow-hidden flex flex-col">
            <Tabs value={activeTab} onValueChange={setActiveTab} className="flex-1 flex flex-col overflow-hidden">
              <div className="border-b border-[#c4b8a0] px-4 shrink-0 bg-[#ede9dd]">
                <TabsList className="bg-transparent h-10 gap-4">
                  <TabsTrigger
                    value="weapons"
                    className="data-[state=active]:bg-transparent data-[state=active]:text-red-700 data-[state=active]:border-b-2 data-[state=active]:border-red-700 rounded-none text-[#8b7355] hover:text-[#5c4a3a] text-xs h-10"
                  >
                    Weapons
                  </TabsTrigger>
                  <TabsTrigger
                    value="defense"
                    className="data-[state=active]:bg-transparent data-[state=active]:text-red-700 data-[state=active]:border-b-2 data-[state=active]:border-red-700 rounded-none text-[#8b7355] hover:text-[#5c4a3a] text-xs h-10"
                  >
                    Defense
                  </TabsTrigger>
                  <TabsTrigger
                    value="gear"
                    className="data-[state=active]:bg-transparent data-[state=active]:text-red-700 data-[state=active]:border-b-2 data-[state=active]:border-red-700 rounded-none text-[#8b7355] hover:text-[#5c4a3a] text-xs h-10"
                  >
                    Gear
                  </TabsTrigger>
                  <TabsTrigger
                    value="spells"
                    className="data-[state=active]:bg-transparent data-[state=active]:text-red-700 data-[state=active]:border-b-2 data-[state=active]:border-red-700 rounded-none text-[#8b7355] hover:text-[#5c4a3a] text-xs h-10"
                  >
                    Spells
                  </TabsTrigger>
                  <TabsTrigger
                    value="pets"
                    className="data-[state=active]:bg-transparent data-[state=active]:text-red-700 data-[state=active]:border-b-2 data-[state=active]:border-red-700 rounded-none text-[#8b7355] hover:text-[#5c4a3a] text-xs h-10"
                  >
                    Pets
                  </TabsTrigger>
                  <TabsTrigger
                    value="details"
                    className="data-[state=active]:bg-transparent data-[state=active]:text-red-700 data-[state=active]:border-b-2 data-[state=active]:border-red-700 rounded-none text-[#8b7355] hover:text-[#5c4a3a] text-xs h-10"
                  >
                    Details
                  </TabsTrigger>
                  <TabsTrigger
                    value="feats"
                    className="data-[state=active]:bg-transparent data-[state=active]:text-red-700 data-[state=active]:border-b-2 data-[state=active]:border-red-700 rounded-none text-[#8b7355] hover:text-[#5c4a3a] text-xs h-10"
                  >
                    Feats
                  </TabsTrigger>
                  <TabsTrigger
                    value="actions"
                    className="data-[state=active]:bg-transparent data-[state=active]:text-red-700 data-[state=active]:border-b-2 data-[state=active]:border-red-700 rounded-none text-[#8b7355] hover:text-[#5c4a3a] text-xs h-10"
                  >
                    Actions
                  </TabsTrigger>
                </TabsList>
              </div>

              <div className="flex-1 overflow-hidden">
                <ScrollArea className="h-full">
                  <div className="p-4">
                    <TabsContent value="weapons" className="mt-0">
                      <WeaponsTab />
                    </TabsContent>

                    <TabsContent value="defense" className="mt-0">
                      <div className="text-[#8b7355] text-sm">View armor, shields, and defensive stats.</div>
                    </TabsContent>

                    <TabsContent value="gear" className="mt-0">
                      <div className="text-[#8b7355] text-sm">No gear equipped</div>
                    </TabsContent>

                    <TabsContent value="spells" className="mt-0">
                      <div className="text-[#8b7355] text-sm">No spells available</div>
                    </TabsContent>

                    <TabsContent value="pets" className="mt-0">
                      <div className="text-[#8b7355] text-sm">No pets or companions</div>
                    </TabsContent>

                    <TabsContent value="details" className="mt-0">
                      <div className="space-y-4">
                        <div>
                          <div className="text-xs text-[#8b7355] uppercase">Ancestry</div>
                          <div>{ancestry}</div>
                        </div>
                        <div>
                          <div className="text-xs text-[#8b7355] uppercase">Background</div>
                          <div>{background}</div>
                        </div>
                        <div>
                          <div className="text-xs text-[#8b7355] uppercase">Class</div>
                          <div>{characterClass}</div>
                        </div>
                      </div>
                    </TabsContent>

                    <TabsContent value="feats" className="mt-0">
                      <FeatsTab level={level} />
                    </TabsContent>

                    <TabsContent value="actions" className="mt-0">
                      <div className="text-[#8b7355] text-sm">No actions available</div>
                    </TabsContent>
                  </div>
                </ScrollArea>
              </div>
            </Tabs>
          </div>
        </div>
      </div>
    </div>
  );
}
