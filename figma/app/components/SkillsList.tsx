import { Circle } from "lucide-react";

interface Skill {
  name: string;
  ability: string;
  proficiency: "untrained" | "trained" | "expert" | "master" | "legendary";
  modifier: number;
}

interface SkillsListProps {
  skills: Skill[];
  onSkillChange: (index: number, proficiency: "untrained" | "trained" | "expert" | "master" | "legendary") => void;
}

export function SkillsList({ skills, onSkillChange }: SkillsListProps) {
  const getProficiencyIcon = (proficiency: string) => {
    switch (proficiency) {
      case "trained":
        return (
          <div className="w-5 h-5 rounded-full bg-[#c4b8a0] flex items-center justify-center">
            <div className="w-2 h-2 rounded-full bg-[#5c4a3a]"></div>
          </div>
        );
      case "expert":
        return (
          <div className="w-5 h-5 rounded-full bg-red-600 flex items-center justify-center">
            <div className="w-2 h-2 rounded-full bg-white"></div>
          </div>
        );
      case "master":
        return (
          <div className="w-5 h-5 rounded-full bg-blue-600 flex items-center justify-center">
            <div className="w-2 h-2 rounded-full bg-white"></div>
          </div>
        );
      case "legendary":
        return (
          <div className="w-5 h-5 rounded-full bg-purple-600 flex items-center justify-center">
            <div className="w-2 h-2 rounded-full bg-white"></div>
          </div>
        );
      default:
        return <Circle className="w-5 h-5 text-gray-400" />;
    }
  };

  return (
    <div className="space-y-1 border-t border-[#c4b8a0] pt-4">
      {skills.map((skill, index) => (
        <div
          key={index}
          className="flex items-center gap-3 py-1 px-2 rounded hover:bg-[#f8f5ec] cursor-pointer transition-colors"
          onClick={() => {
            const proficiencies = ["untrained", "trained", "expert", "master", "legendary"] as const;
            const currentIndex = proficiencies.indexOf(skill.proficiency);
            const nextIndex = (currentIndex + 1) % proficiencies.length;
            onSkillChange(index, proficiencies[nextIndex]);
          }}
        >
          {getProficiencyIcon(skill.proficiency)}
          <div className="flex items-center gap-2 min-w-[60px]">
            <div className="w-6 h-6 rounded-full bg-[#d8d0bf] flex items-center justify-center">
              <span className="text-xs text-[#5c4a3a]">{skill.ability.charAt(0)}</span>
            </div>
            <span className="text-sm w-8">
              {skill.modifier >= 0 ? `+${skill.modifier}` : skill.modifier}
            </span>
          </div>
          <span className="text-sm text-[#5c4a3a]">{skill.name}</span>
        </div>
      ))}
    </div>
  );
}
