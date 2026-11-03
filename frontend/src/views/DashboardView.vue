<script setup>
import { computed, onMounted, ref, watch } from 'vue'
import { storeToRefs } from 'pinia'
import { useTicketsStore } from '../stores/tickets'
import MorningBrief from '../components/MorningBrief.vue'
import TicketFilters from '../components/TicketFilters.vue'
import TicketTable from '../components/TicketTable.vue'
import TicketFormModal from '../components/TicketFormModal.vue'
import ConfirmDialog from '../components/ConfirmDialog.vue'

const store = useTicketsStore()
const { tickets, stats, filters, loadingList, saving, error } = storeToRefs(store)

const showCreate = ref(false)
const editingTicket = ref(null)
const deletingTicket = ref(null)
let searchTimer

const filterModel = computed({
  get: () => filters.value,
  set: (value) => store.setFilters(value)
})

watch(
  filters,
  (value, oldValue) => {
    const searchChanged = value.search !== oldValue?.search
    if (searchChanged) {
      clearTimeout(searchTimer)
      searchTimer = setTimeout(() => {
        store.fetchTickets()
      }, 250)
      return
    }
    store.fetchTickets()
  },
  { deep: true }
)

onMounted(async () => {
  await Promise.all([store.fetchTickets(), store.fetchStats()])
})

function focusAttention() {
  store.setFilters({
    needsAttention: !filters.value.needsAttention,
    status: '',
    priority: '',
    assignee: '',
    search: ''
  })
}

async function handleCreate(payload) {
  await store.createTicket(payload)
  showCreate.value = false
}

async function handleEdit(payload) {
  await store.updateTicket(editingTicket.value.id, {
    ...payload,
    actor: 'Support Manager'
  })
  editingTicket.value = null
}

async function confirmDelete() {
  if (!deletingTicket.value) return
  await store.deleteTicket(deletingTicket.value.id)
  deletingTicket.value = null
}

function clearFilters() {
  store.clearFilters()
}
</script>

<template>
  <div class="space-y-6">
    <header class="flex flex-wrap items-end justify-between gap-4">
      <div>
        <p class="text-xs font-semibold uppercase tracking-[0.18em] text-tide-700">Support desk</p>
        <h1 class="mt-1 font-display text-4xl tracking-tight text-ink-950 sm:text-5xl">
          Magentrix Desk
        </h1>
        <p class="mt-2 max-w-xl text-sm text-ink-600 sm:text-base">
          A morning-ready queue for support managers — triage fast, leave a trail, keep customers moving.
        </p>
      </div>
      <button type="button" class="btn-primary" @click="showCreate = true">New ticket</button>
    </header>

    <MorningBrief
      :stats="stats"
      :active-focus="filters.needsAttention"
      @focus-attention="focusAttention"
    />

    <p
      v-if="error"
      class="rounded-xl border border-rose-200 bg-rose-50 px-4 py-3 text-sm text-rose-800"
    >
      {{ error }}
    </p>

    <TicketFilters v-model="filterModel" @clear="clearFilters" />

    <div class="flex items-center justify-between gap-3 text-sm text-ink-500">
      <p>
        Showing
        <span class="font-semibold text-ink-800">{{ tickets.length }}</span>
        ticket{{ tickets.length === 1 ? '' : 's' }}
        <span v-if="filters.needsAttention" class="text-tide-700"> · attention focus on</span>
      </p>
      <p v-if="stats">{{ stats.total }} total in system</p>
    </div>

    <TicketTable
      :tickets="tickets"
      :loading="loadingList"
      @edit="editingTicket = $event"
      @delete="deletingTicket = $event"
    />

    <TicketFormModal
      :open="showCreate"
      mode="create"
      :busy="saving"
      @close="showCreate = false"
      @submit="handleCreate"
    />

    <TicketFormModal
      :open="!!editingTicket"
      mode="edit"
      :ticket="editingTicket"
      :busy="saving"
      @close="editingTicket = null"
      @submit="handleEdit"
    />

    <ConfirmDialog
      :open="!!deletingTicket"
      title="Delete this ticket?"
      :message="
        deletingTicket
          ? `“${deletingTicket.title}” and its comments will be permanently removed.`
          : ''
      "
      confirm-label="Delete ticket"
      :busy="saving"
      @cancel="deletingTicket = null"
      @confirm="confirmDelete"
    />
  </div>
</template>
