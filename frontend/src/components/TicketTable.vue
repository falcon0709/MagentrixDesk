<script setup>
import { useRouter } from 'vue-router'
import PriorityBadge from './PriorityBadge.vue'
import StatusBadge from './StatusBadge.vue'

defineProps({
  tickets: { type: Array, default: () => [] },
  loading: { type: Boolean, default: false }
})

const emit = defineEmits(['edit', 'delete'])
const router = useRouter()

function formatDate(value) {
  return new Intl.DateTimeFormat(undefined, {
    month: 'short',
    day: 'numeric',
    hour: 'numeric',
    minute: '2-digit'
  }).format(new Date(value))
}

function openTicket(id) {
  router.push({ name: 'ticket-detail', params: { id } })
}
</script>

<template>
  <div class="panel overflow-hidden">
    <div v-if="loading" class="space-y-3 p-5">
      <div v-for="n in 5" :key="n" class="h-14 animate-pulse rounded-xl bg-ink-100/80" />
    </div>

    <div v-else-if="!tickets.length" class="px-6 py-16 text-center">
      <p class="font-display text-2xl text-ink-900">No tickets match this view</p>
      <p class="mt-2 text-sm text-ink-500">
        Try clearing filters, or create a ticket to populate the queue.
      </p>
    </div>

    <div v-else class="overflow-x-auto">
      <table class="min-w-full text-left text-sm">
        <thead class="border-b border-ink-100 bg-ink-50/70 text-xs uppercase tracking-wide text-ink-500">
          <tr>
            <th class="px-4 py-3 font-semibold">Ticket</th>
            <th class="px-4 py-3 font-semibold">Customer</th>
            <th class="px-4 py-3 font-semibold">Status</th>
            <th class="px-4 py-3 font-semibold">Priority</th>
            <th class="px-4 py-3 font-semibold">Assignee</th>
            <th class="px-4 py-3 font-semibold">Age</th>
            <th class="px-4 py-3 font-semibold">Updated</th>
            <th class="px-4 py-3 font-semibold text-right">Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="ticket in tickets"
            :key="ticket.id"
            class="cursor-pointer border-b border-ink-100/80 transition hover:bg-tide-50/40"
            @click="openTicket(ticket.id)"
          >
            <td class="max-w-xs px-4 py-3">
              <div class="flex items-start gap-2">
                <span class="mt-0.5 font-mono text-xs text-ink-400">#{{ ticket.id }}</span>
                <div>
                  <p class="font-semibold text-ink-900">{{ ticket.title }}</p>
                  <p class="mt-0.5 line-clamp-1 text-xs text-ink-500">{{ ticket.description }}</p>
                </div>
              </div>
            </td>
            <td class="px-4 py-3">
              <p class="font-medium text-ink-800">{{ ticket.customerName }}</p>
              <p class="text-xs text-ink-500">{{ ticket.customerEmail }}</p>
            </td>
            <td class="px-4 py-3"><StatusBadge :status="ticket.status" /></td>
            <td class="px-4 py-3"><PriorityBadge :priority="ticket.priority" /></td>
            <td class="px-4 py-3">
              <span v-if="ticket.assignee" class="text-ink-800">{{ ticket.assignee }}</span>
              <span v-else class="rounded-full bg-amber-50 px-2 py-0.5 text-xs font-semibold text-amber-800">
                Unassigned
              </span>
            </td>
            <td class="px-4 py-3">
              <span
                class="font-medium"
                :class="ticket.ageInDays >= 3 && ticket.status < 2 ? 'text-rose-700' : 'text-ink-700'"
              >
                {{ ticket.ageInDays }}d
              </span>
            </td>
            <td class="whitespace-nowrap px-4 py-3 text-ink-600">{{ formatDate(ticket.updatedAt) }}</td>
            <td class="px-4 py-3">
              <div class="flex justify-end gap-2" @click.stop>
                <button type="button" class="btn-secondary px-3 py-1.5 text-xs" @click="openTicket(ticket.id)">
                  View
                </button>
                <button type="button" class="btn-secondary px-3 py-1.5 text-xs" @click="$emit('edit', ticket)">
                  Edit
                </button>
                <button type="button" class="btn-danger px-3 py-1.5 text-xs" @click="$emit('delete', ticket)">
                  Delete
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
