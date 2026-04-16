import { useState } from "react";
import { Button } from "./ui/button";
import { Input } from "./ui/input";
import { Badge } from "./ui/badge";
import { Plus, X } from "lucide-react";
import { Label } from "./ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue
} from "./ui/select";

interface Feat {
  name: string;
  level: number;
  type: string;
}

interface FeatsSectionProps {
  feats: Feat[];
  onAddFeat: (feat: Feat) => void;
  onRemoveFeat: (index: number) => void;
}

const featTypes = [
  "Ancestry",
  "Class",
  "General",
  "Skill",
  "Archetype",
  "Dedication"
];

export function FeatsSection({ feats, onAddFeat, onRemoveFeat }: FeatsSectionProps) {
  const [newFeatName, setNewFeatName] = useState("");
  const [newFeatLevel, setNewFeatLevel] = useState(1);
  const [newFeatType, setNewFeatType] = useState("General");

  const handleAddFeat = () => {
    if (newFeatName.trim()) {
      onAddFeat({
        name: newFeatName,
        level: newFeatLevel,
        type: newFeatType
      });
      setNewFeatName("");
      setNewFeatLevel(1);
      setNewFeatType("General");
    }
  };

  return (
    <div className="space-y-4">
      <div className="border rounded-lg p-4 space-y-3">
        <Label>Add New Feat</Label>
        <div className="grid grid-cols-1 md:grid-cols-4 gap-3">
          <Input
            placeholder="Feat name"
            value={newFeatName}
            onChange={(e) => setNewFeatName(e.target.value)}
            className="md:col-span-2"
          />
          <Select value={newFeatType} onValueChange={setNewFeatType}>
            <SelectTrigger>
              <SelectValue />
            </SelectTrigger>
            <SelectContent>
              {featTypes.map((type) => (
                <SelectItem key={type} value={type}>
                  {type}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
          <Input
            type="number"
            min="1"
            max="20"
            value={newFeatLevel}
            onChange={(e) => setNewFeatLevel(parseInt(e.target.value) || 1)}
          />
        </div>
        <Button onClick={handleAddFeat} className="w-full">
          <Plus className="w-4 h-4 mr-2" />
          Add Feat
        </Button>
      </div>

      <div className="space-y-2">
        {feats.length === 0 ? (
          <div className="text-center py-8 text-gray-500">
            No feats added yet
          </div>
        ) : (
          feats.map((feat, index) => (
            <div
              key={index}
              className="flex items-center justify-between p-3 border rounded-lg hover:bg-gray-50"
            >
              <div className="flex items-center gap-3">
                <span>{feat.name}</span>
                <Badge variant="outline">{feat.type}</Badge>
                <Badge variant="secondary">Level {feat.level}</Badge>
              </div>
              <Button
                size="sm"
                variant="ghost"
                onClick={() => onRemoveFeat(index)}
              >
                <X className="w-4 h-4" />
              </Button>
            </div>
          ))
        )}
      </div>
    </div>
  );
}
