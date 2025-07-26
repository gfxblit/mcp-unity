---
description: Git Commit Workflow
---

// turbo-all

## Purpose
A fully automated, end-to-end workflow that to ensure all git changes are reviewed and structured correctly using the Conventional Commits specification. After invoking `/commit`, Cascade can follow these steps without any additional guidance.

---
### Step 1: Verify repository status
1. Run `git status` to list modified, staged, and untracked files.
2. If **no** modifications are detected, STOP the workflow and inform the user that there is nothing to commit.

---
### Step 2: Stage all edits
1. Run `git add .` to stage every modified or untracked file.

---
### Step 3: Review staged diff
1. Run `git diff --staged --word-diff` and capture the output.
2. Use the diff to build a concise summary of the changes (this will become the commit message body).

---
### Step 4: Generate commit message
1. Determine whether the commit is **single-scope** or **multi-scope**:
   • If only one logical change is present → *single-scope*.
   • Otherwise → *multi-scope*.
2. Construct the header(s) using `<type>(scope): <description>` following Conventional Commits.
3. Draft an **optional body** summarising broader impact, listing bullet points for major file or API impacts.
4. Append **optional footers** (`BREAKING CHANGE:`, `Closes #X`, etc.).

---
### Step 5: Present commit for approval
1. Show the full commit message to the user inside a fenced `markdown` block, *exactly as it would appear*.
2. Ask **“Ready to commit? (yes/no)”**.
   • If **yes** → continue to step 6.
   • If **no** → request user corrections, regenerate message, then repeat this step.

---
### Rules & Conventions
* Always reference **filenames only** (e.g. `MatchState.cs`) in the message body; never include full paths.
* Use the following **commit types**: `feat`, `fix`, `build`, `chore`, `ci`, `docs`, `perf`, `refactor`, `revert`, `style`, `test`.
* Keep each header line ≤ 86 characters.
* Avoid passive voice; write in imperative mood (“add”, “fix”, “remove”, etc.).
* When multiple headers are present, separate them with a newline and place the body after a blank line.

---
### Example (multi-scope)
```markdown
feat(match): add overtime logic
fix(ui): prevent score overlay flicker

Adds sudden-death overtime to `MatchState.cs` and resolves an intermittent UI flicker in `ScoreOverlay.cs`.

Closes #456
```

---
**End of workflow – no further instructions required.**