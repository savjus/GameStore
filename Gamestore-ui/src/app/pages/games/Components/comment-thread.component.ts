import { CommonModule } from '@angular/common';
import { Component, EventEmitter, forwardRef, Input, Output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { GameComment } from '../../../core/models/gameComment';

@Component({
  selector: 'app-comment-thread',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule, forwardRef(() => CommentThreadComponent)],
  templateUrl: './comment-thread.component.html',
  styleUrl: './comment-thread.component.scss',
})
export class CommentThreadComponent {
  @Input() comments: GameComment[] = [];
  @Input() depth = 0;

  @Output() reply = new EventEmitter<GameComment>();
  @Output() quote = new EventEmitter<GameComment>();
  @Output() delete = new EventEmitter<string>();
  @Output() ban = new EventEmitter<GameComment>();
}