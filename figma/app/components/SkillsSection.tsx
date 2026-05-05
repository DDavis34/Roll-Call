import { Badge } from "./ui/badge";
import { Button } from "./ui/button";
import { ScrollArea } from "./ui/scroll-area";

interface Skill {
  name: string;
  ability: string;
  proficiency: "untrained" | "trained" | "expert" | "master" | "legendary";
}

interface SkillsSectionProps {
  skills: Skill[];
  onSkillChange: (index: number, proficiency: Skill["proficiency"]) => void;
}

const proficiencyLevels: Skill["proficiency"][] = [
  "untrained",
  "trained",
  "expert",
  "master",
  "legendary"
];

const proficiencyColors = {
  untrained: "bg-gray-200 text-gray-700",
  trained: "bg-green-200 text-green-700",
  expert: "bg-blue-200 text-blue-700",
  master: "bg-purple-200 text-purple-700",
  legendary: "bg-amber-200 text-amber-700"
};

export function SkillsSection({ skills, onSkillChange }: SkillsSectionProps) {
  return (
    <ScrollArea className="h-[400px] pr-4">
      <div className="space-y-2">
        {skills.map((skill, index) => (
          <div
            key={skill.name}
            className="flex items-center justify-between p-3 border rounded-lg hover:bg-gray-50"
          >
            <div className="flex-1">
              <div className="flex items-center gap-2">
                <span>{skill.name}</span>
                <Badge variant="outline" className="text-xs">
                  {skill.ability}
                </Badge>
              </div>
            </div>

            <div className="flex gap-1">
              {proficiencyLevels.map((level) => (
                <Button
                  key={level}
                  size="sm"
                  variant={skill.proficiency === level ? "default" : "outline"}
                  className={
                    skill.proficiency === level
                      ? `${proficiencyColors[level]} hover:opacity-80`
                      : ""
                  }
                  onClick={() => onSkillChange(index, level)}
                >
                  {level[0].toUpperCase()}
                </Button>
              ))}
            </div>
          </div>
        ))}
      </div>
    </ScrollArea>
  );
}
