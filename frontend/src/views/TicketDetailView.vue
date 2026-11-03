<script setup>
import { computed, onMounted, ref, watch } from 'vue'
import { useRouter } from 'vue-router'
import { storeToRefs } from 'pinia'
import { useTicketsStore } from '../stores/tickets'
import PriorityBadge from '../components/PriorityBadge.vue'
import StatusBadge from '../components/StatusBadge.vue'
import TicketFormModal from '../components/TicketFormModal.vue'
import ConfirmDialog from '../components/ConfirmDialog.vue'
import { STATUS_LABELS } from '../api/client'

const props = defineProps({
  id: { type: [String, Number], required: true }
})

const router = useRouter()
const store = useTicketsStore()
const { currentTicket, loadingDetail, saving, error } = storeToRefs(store)

const showEdit = ref(false)
const showDelete = ref(false)
const commentText = ref('')
const commentAuthor = ref('Support Manager')
const commentError = ref('')

const ticketId = computed(() => Number(props.id))

async function load() {
  await store.fetchTicket(ticketId.value)
}

onMounted(load)
watch(ticketId, load)

function formatDate(value) {
  return new Intl.DateTimeFormat(undefined, {
    dateStyle: 'medium',
    timeStyle: 'short'
  }).format(new Date(value))
}

function historyLabel(entry) {
  if (entry.action === 'Created') return 'Ticket created'
  if (entry.action === 'CommentAdded') return 'Comment added'
  if (entry.field) {
    return `${entry.field}: ${entry.oldValue ?? '—'} → ${entry.newValue ?? '—'}`
  }
  return entry.action
}

async function quickStatus(status) {
  if (!currentTicket.value) return
  const t = currentTicket.value
  await store.updateTicket(t.id, {
    title: t.title,
    description: t.description,
    status,
    priority: t.priority,
    customerName: t.customerName,
    customerEmail: t.customerEmail,
    assignee: t.assignee,
    actor: 'Support Manager'
  })
}

async function saveEdit(payload) {
  await store.updateTicket(ticketId.value, { ...payload, actor: 'Support Manager' })
  showEdit.value = false
}

async function confirmDelete() {
  await store.deleteTicket(ticketId.value)
  showDelete.value = false
  router.push({ name: 'dashboard' })
}

async function submitComment() {
  commentError.value = ''
  if (!commentText.value.trim()) {
    commentError.value = 'Comment text is required.'
    return
  }
  await store.addComment(ticketId.value, {
    text: commentText.value.trim(),
    author: commentAuthor.value.trim() || 'Support Agent'
  })
  commentText.value = ''
}
</script>

<template>
  <div class="space-y-6">
    <button type="button" class="btn-secondary" @click="router.push({ name: 'dashboard' })">
      ← Back to dashboard
    </button>

    <div v-if="loadingDetail" class="panel space-y-4 p-6">
      <div class="h-8 w-2/3 animate-pulse rounded bg-ink-100" />
      <div class="h-24 animate-pulse rounded bg-ink-100" />
      <div class="h-40 animate-pulse rounded bg-ink-100" />
    </div>

    <p
      v-else-if="error"
      class="rounded-xl border border-rose-200 bg-rose-50 px-4 py-3 text-sm text-rose-800"
    >
      {{ error }}
    </p>

    <template v-else-if="currentTicket">
      <section class="panel overflow-hidden">
        <div class="border-b border-ink-100 bg-gradient-to-r from-ink-900 to-tide-800 px-6 py-6 text-white">
          <div class="flex flex-wrap items-start justify-between gap-4">
            <div>
              <p class="font-mono text-xs text-tide-100">Ticket #{{ currentTicket.id }}</p>
              <h1 class="mt-2 max-w-3xl font-display text-3xl tracking-tight sm:text-4xl">
                {{ currentTicket.title }}
              </h1>
              <div class="mt-4 flex flex-wrap items-center gap-2">
                <StatusBadge :status="currentTicket.status" />
                <PriorityBadge :priority="currentTicket.priority" />
                <span class="rounded-full bg-white/10 px-2.5 py-0.5 text-xs font-semibold text-white/90">
                  Age {{ currentTicket.ageInDays }}d
                </span>
              </div>
            </div>
            <div class="flex flex-wrap gap-2">
              <button type="button" class="btn-secondary" @click="showEdit = true">Edit</button>
              <button type="button" class="btn-danger" @click="showDelete = true">Delete</button>
            </div>
          </div>
        </div>

        <div class="grid gap-6 p-6 lg:grid-cols-[1.4fr_0.8fr]">
          <div class="space-y-5">
            <div>
              <h2 class="text-xs font-semibold uppercase tracking-wide text-ink-500">Description</h2>
              <p class="mt-2 whitespace-pre-wrap text-sm leading-relaxed text-ink-800">
                {{ currentTicket.description }}
              </p>
            </div>

            <div>
              <h2 class="text-xs font-semibold uppercase tracking-wide text-ink-500">Quick status</h2>
              <div class="mt-2 flex flex-wrap gap-2">
                <button
                  v-for="(label, value) in STATUS_LABELS"
                  :key="value"
                  type="button"
                  class="rounded-full border px-3 py-1 text-xs font-semibold transition"
                  :class="
                    currentTicket.status === Number(value)
                      ? 'border-tide-600 bg-tide-600 text-white'
                      : 'border-ink-200 bg-white text-ink-700 hover:bg-ink-50'
                  "
                  :disabled="saving || currentTicket.status === Number(value)"
                  @click="quickStatus(Number(value))"
                >
                  {{ label }}
                </button>
              </div>
            </div>
          </div>

          <aside class="space-y-4 rounded-2xl bg-ink-50/80 p-4">
            <div>
              <p class="label">Customer</p>
              <p class="font-semibold text-ink-900">{{ currentTicket.customerName }}</p>
              <p class="text-sm text-ink-600">{{ currentTicket.customerEmail }}</p>
            </div>
            <div>
              <p class="label">Assignee</p>
              <p class="text-sm font-medium text-ink-800">
                {{ currentTicket.assignee || 'Unassigned' }}
              </p>
            </div>
            <div class="grid grid-cols-2 gap-3 text-sm">
              <div>
                <p class="label">Created</p>
                <p class="text-ink-800">{{ formatDate(currentTicket.createdAt) }}</p>
              </div>
              <div>
                <p class="label">Updated</p>
                <p class="text-ink-800">{{ formatDate(currentTicket.updatedAt) }}</p>
              </div>
            </div>
          </aside>
        </div>
      </section>

      <div class="grid gap-6 lg:grid-cols-2">
        <section class="panel p-5">
          <h2 class="font-display text-2xl text-ink-950">Comments</h2>
          <p class="mt-1 text-sm text-ink-500">Internal notes and customer replies for this ticket.</p>

          <form class="mt-4 space-y-3" @submit.prevent="submitComment">
            <div>
              <label class="label" for="author">Author</label>
              <input id="author" v-model="commentAuthor" class="field" maxlength="120" />
            </div>
            <div>
              <label class="label" for="comment">Comment</label>
              <textarea
                id="comment"
                v-model="commentText"
                class="field min-h-[90px]"
                maxlength="2000"
                placeholder="Add context, next steps, or a customer update…"
              />
            </div>
            <p v-if="commentError" class="text-sm text-rose-700">{{ commentError }}</p>
            <button type="submit" class="btn-primary" :disabled="saving">
              {{ saving ? 'Posting…' : 'Add comment' }}
            </button>
          </form>

          <ul class="mt-6 space-y-3">
            <li
              v-for="comment in currentTicket.comments"
              :key="comment.id"
              class="rounded-xl border border-ink-100 bg-white p-4"
            >
              <div class="flex items-center justify-between gap-3">
                <p class="text-sm font-semibold text-ink-900">{{ comment.author }}</p>
                <p class="text-xs text-ink-500">{{ formatDate(comment.createdAt) }}</p>
              </div>
              <p class="mt-2 whitespace-pre-wrap text-sm text-ink-700">{{ comment.text }}</p>
            </li>
            <li v-if="!currentTicket.comments.length" class="py-6 text-center text-sm text-ink-500">
              No comments yet — be the first to leave a note.
            </li>
          </ul>
        </section>

        <section class="panel p-5">
          <h2 class="font-display text-2xl text-ink-950">History</h2>
          <p class="mt-1 text-sm text-ink-500">Field-level audit trail for status, ownership, and edits.</p>

          <ol class="relative mt-6 space-y-4 border-l border-ink-200 pl-5">
            <li v-for="entry in currentTicket.history" :key="entry.id" class="relative">
              <span class="absolute -left-[1.55rem] top-1.5 h-2.5 w-2.5 rounded-full bg-tide-500 ring-4 ring-white" />
              <p class="text-sm font-semibold text-ink-900">{{ historyLabel(entry) }}</p>
              <p class="mt-1 text-xs text-ink-500">
                {{ entry.actor }} · {{ formatDate(entry.createdAt) }}
              </p>
            </li>
            <li v-if="!currentTicket.history.length" class="text-sm text-ink-500">No history recorded.</li>
          </ol>
        </section>
      </div>
    </template>

    <TicketFormModal
      :open="showEdit"
      mode="edit"
      :ticket="currentTicket"
      :busy="saving"
      @close="showEdit = false"
      @submit="saveEdit"
    />

    <ConfirmDialog
      :open="showDelete"
      title="Delete this ticket?"
      message="This permanently removes the ticket, comments, and history."
      confirm-label="Delete ticket"
      :busy="saving"
      @cancel="showDelete = false"
      @confirm="confirmDelete"
    />
  </div>
</template>
