<script setup>
defineProps({
  stats: { type: Object, default: null },
  activeFocus: { type: Boolean, default: false }
})

defineEmits(['focus-attention'])
</script>

<template>
  <section class="panel overflow-hidden">
    <div class="border-b border-ink-100 bg-gradient-to-r from-tide-700 via-tide-600 to-ink-700 px-6 py-5 text-white">
      <p class="text-xs font-semibold uppercase tracking-[0.18em] text-tide-100">Morning brief</p>
      <h2 class="mt-1 font-display text-2xl tracking-tight sm:text-3xl">
        What needs a manager’s eye today
      </h2>
      <p class="mt-2 max-w-2xl text-sm text-tide-50/90">
        High-priority open work, aging unassigned tickets, and today’s movement — so you start with signal, not noise.
      </p>
    </div>

    <div class="grid gap-3 p-4 sm:grid-cols-2 lg:grid-cols-4 lg:p-5">
      <div class="rounded-xl bg-ink-50/80 p-4">
        <p class="text-xs font-semibold uppercase tracking-wide text-ink-500">Open queue</p>
        <p class="mt-2 font-display text-3xl text-ink-950">{{ stats?.open ?? '—' }}</p>
        <p class="mt-1 text-xs text-ink-500">{{ stats?.inProgress ?? 0 }} in progress</p>
      </div>
      <div class="rounded-xl bg-rose-50/70 p-4">
        <p class="text-xs font-semibold uppercase tracking-wide text-rose-700">High priority active</p>
        <p class="mt-2 font-display text-3xl text-rose-900">{{ stats?.highPriorityOpen ?? '—' }}</p>
        <p class="mt-1 text-xs text-rose-700/80">Open or in progress</p>
      </div>
      <div class="rounded-xl bg-amber-50/80 p-4">
        <p class="text-xs font-semibold uppercase tracking-wide text-amber-800">Aging unassigned</p>
        <p class="mt-2 font-display text-3xl text-amber-950">{{ stats?.agingUnassigned ?? '—' }}</p>
        <p class="mt-1 text-xs text-amber-800/80">Open &gt; 3 days, no owner</p>
      </div>
      <div class="rounded-xl bg-tide-50/80 p-4">
        <p class="text-xs font-semibold uppercase tracking-wide text-tide-800">Touched today</p>
        <p class="mt-2 font-display text-3xl text-tide-950">{{ stats?.updatedToday ?? '—' }}</p>
        <p class="mt-1 text-xs text-tide-800/80">Updated since midnight UTC</p>
      </div>
    </div>

    <div class="flex flex-wrap items-center justify-between gap-3 border-t border-ink-100 px-5 py-4">
      <p class="text-sm text-ink-600">
        Tip: use <span class="font-semibold text-ink-800">Needs attention</span> to jump straight to high priority,
        unassigned, or aging tickets.
      </p>
      <button
        type="button"
        class="btn-primary"
        :class="activeFocus ? 'ring-2 ring-tide-300 ring-offset-2' : ''"
        @click="$emit('focus-attention')"
      >
        {{ activeFocus ? 'Showing attention queue' : 'Focus: needs attention' }}
      </button>
    </div>
  </section>
</template>
