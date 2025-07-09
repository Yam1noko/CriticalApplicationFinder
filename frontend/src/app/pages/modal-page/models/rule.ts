export interface Rule {
  id?: number;
  name: string;
  isActive: boolean;
  useAnd: boolean;
  ruleFullNames: string[];
  ruleSubstrings: string[];

  ruleFullNamesStr?: string;
  ruleSubstringsStr?: string;
}
