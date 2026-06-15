import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Game } from '../../core/models/game';
import { GameService } from '../../core/services/game.service';
import { CommentService, AddCommentRequest, BanRequest } from '../../core/services/comment.service';
import { OrderService } from '../../core/services/order.service';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDividerModule } from '@angular/material/divider';
import { GameComment } from '../../core/models/gameComment';
import { CommentThreadComponent } from './Components/comment-thread.component';

@Component({
  selector: 'app-game-detail',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDividerModule,
    CommentThreadComponent,
  ],
  templateUrl: './game-detail.page.html',
  styleUrl: './game-detail.page.scss',
})
export class GameDetailPage implements OnInit {
  game?: Game;
  loadingGames = false;
  loadingComments = false;
  downloading = false;
  errorMessage = '';
  key = '';

  comments: GameComment[] = [];

  // Add comment form
  newName = '';
  newBody = '';
  parentId: string | null = null;
  action: string | null = null;

  /** Shown above the form to indicate context. */
  replyContext: { type: 'reply' | 'quote'; authorName: string; body?: string } | null = null;

  submitting = false;
  commentErrorMessage = '';

  // Ban
  banDurations: string[] = [];
  banTargetName = '';
  selectedDuration = '';
  banning = false;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly gameService: GameService,
    private readonly commentService: CommentService,
    private readonly orderService: OrderService,
    private readonly changeDetector: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.key = this.route.snapshot.paramMap.get('key') ?? '';
    if (!this.key) {
      this.errorMessage = 'Missing game key.';
      return;
    }
    this.loadGame();
    this.loadComments();
    this.loadBanDurations();
  }

  loadGame(): void {
    this.loadingGames = true;
    this.gameService.getByKey(this.key).subscribe({
      next: (game) => {
        this.game = game;
        this.loadingGames = false;
        this.changeDetector.markForCheck();
      },
      error: () => {
        this.errorMessage = 'Failed to load game.';
        this.loadingGames = false;
      },
    });
  }

  loadComments(): void {
    this.loadingComments = true;
    this.commentService.getByGameKey(this.key).subscribe({
      next: (comments: GameComment[]) => {
        this.comments = comments;
        this.loadingComments = false;
        this.changeDetector.markForCheck();
      },
      error: () => {
        this.loadingComments = false;
      },
    });
  }

  loadBanDurations(): void {
    this.commentService.getBanDurations().subscribe({
      next: (durations) => {
        this.banDurations = durations;
      },
    });
  }

  // ── Reply / Quote ────────────────────────────────────────────────────────────

  setReply(comment: GameComment): void {
    this.parentId = comment.id;
    this.action = 'reply';
    this.replyContext = { type: 'reply', authorName: comment.name };
    this.scrollToForm();
  }

  setQuote(comment: GameComment): void {
    this.parentId = comment.id;
    this.action = 'quote';
    this.replyContext = { type: 'quote', authorName: comment.name, body: comment.body };
    this.scrollToForm();
  }

  clearParent(): void {
    this.parentId = null;
    this.action = null;
    this.replyContext = null;
  }

  private scrollToForm(): void {
    // Give Angular a tick to render the context banner before scrolling
    setTimeout(() => {
      document.getElementById('add-comment-form')?.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }, 50);
  }

  // ── Submit ───────────────────────────────────────────────────────────────────

  submitComment(): void {
    if (!this.newName.trim() || !this.newBody.trim()) return;

    this.submitting = true;
    this.commentErrorMessage = '';
    const request: AddCommentRequest = {
      comment: { name: this.newName.trim(), body: this.newBody.trim() },
      parentId: this.parentId,
      action: this.action,
    };

    this.commentService.addComment(this.key, request).subscribe({
      next: () => {
        this.newName = '';
        this.newBody = '';
        this.clearParent();
        this.submitting = false;
        this.commentErrorMessage = '';
        this.loadComments();
      },
      error: (err) => {
        this.submitting = false;
        // 403 is the conventional status for a banned user; fall back to the
        // server's message or a generic string if nothing more specific comes back.
        const serverMessage = err?.error?.message ?? err?.error ?? null;
        if (err?.status === 403 || (typeof serverMessage === 'string' && serverMessage.toLowerCase().includes('ban'))) {
          this.commentErrorMessage = `"${this.newName}" is banned and cannot post comments.`;
        } else {
          this.commentErrorMessage = 'Failed to post comment. Please try again.';
        }
        this.changeDetector.markForCheck();
      },
    });
  }

  // ── Delete ───────────────────────────────────────────────────────────────────

  deleteComment(id: string): void {
    this.commentService.deleteComment(this.key, id).subscribe({
      next: () => this.loadComments(),
    });
  }

  // ── Ban ──────────────────────────────────────────────────────────────────────

  openBan(comment: GameComment): void {
    this.banTargetName = comment.name;
    this.selectedDuration = '';
  }

  submitBan(): void {
    if (!this.banTargetName || !this.selectedDuration) return;

    this.banning = true;
    const request: BanRequest = { user: this.banTargetName, duration: this.selectedDuration };

    this.commentService.banUser(request).subscribe({
      next: () => {
        this.banTargetName = '';
        this.selectedDuration = '';
        this.banning = false;
        this.changeDetector.markForCheck();
      },
      error: () => {
        this.banning = false;
        this.changeDetector.markForCheck();
      },
    });
  }

  cancelBan(): void {
    this.banTargetName = '';
    this.selectedDuration = '';
  }

  // ── Download / Cart ──────────────────────────────────────────────────────────

  downloadFile(): void {
    if (!this.key) return;
    this.downloading = true;
    this.gameService.downloadFile(this.key).subscribe({
      next: (blob: Blob) => {
        const url = window.URL.createObjectURL(blob);
        const anchor = document.createElement('a');
        anchor.href = url;
        anchor.download = `${this.key}.txt`;
        anchor.click();
        window.URL.revokeObjectURL(url);
        this.downloading = false;
      },
      error: () => {
        this.errorMessage = 'Failed to download the game file.';
        this.downloading = false;
      },
    });
  }

  addToCart(): void {
    this.orderService.buyGame(this.key).subscribe();
  }
}