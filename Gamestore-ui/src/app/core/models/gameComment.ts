export interface GameComment {
  id: string;
  name: string;
  body: string;
  childComments: GameComment[];
}